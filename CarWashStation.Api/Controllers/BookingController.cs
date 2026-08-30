using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarWashStation.Data;
using CarWashStation.Models;
using CarWashStation.Services;

namespace CarWashStation.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISmsService _smsService;
        private readonly IEmailService _emailService;

        public BookingController(ApplicationDbContext context, ISmsService smsService, IEmailService emailService)
        {
            _context = context;
            _smsService = smsService;
            _emailService = emailService;
        }

        [HttpGet("available-slots")]
        public async Task<IActionResult> GetAvailableSlots(DateTime date)
        {
            // Don't show slots for past dates
            if (date.Date < DateTime.Today)
            {
                return Json(new List<string>());
            }

            // Closed on Saturday and Sunday
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                return Json(new List<string>());
            }

            // Working hours 8 AM to 4:00 PM (8:00, 8:30, ..., 16:00)
            var allSlots = Enumerable.Range(0, 17)
                .Select(i => new TimeSpan(8, 0, 0).Add(TimeSpan.FromMinutes(i * 30)))
                .ToList();

            // Filter out past slots for today
            if (date.Date == DateTime.Today)
            {
                var now = DateTime.Now.TimeOfDay;
                allSlots = allSlots.Where(s => s > now).ToList();
            }

            var bookedSlots = await _context.Bookings
                .Where(b => b.BookingDate.Date == date.Date)
                .Select(b => b.TimeSlot)
                .ToListAsync();

            var blockedSlots = await _context.BlockedSlots
                .Where(b => b.Date.Date == date.Date)
                .ToListAsync();

            // If full day is blocked, return empty
            if (blockedSlots.Any(b => b.IsFullDay))
            {
                return Json(new List<string>());
            }

            var blockedTimeSlots = blockedSlots
                .Where(b => b.TimeSlot.HasValue)
                .Select(b => b.TimeSlot!.Value)
                .ToList();

            var availableSlots = allSlots
                .Where(s => !bookedSlots.Contains(s) && !blockedTimeSlots.Contains(s))
                .Select(s => s.ToString(@"hh\:mm"))
                .ToList();

            return Json(availableSlots);
        }

        [HttpGet("services")]
        public async Task<IActionResult> GetServices()
        {
            var services = await _context.Services.ToListAsync();
            return Json(services);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
        {
            if (ModelState.IsValid)
            {
                var selectedService = await _context.Services.FindAsync(booking.ServiceId);
                if (selectedService is null)
                {
                    return BadRequest("Den valda tjänsten finns inte.");
                }

                // Extra server-side check for past dates/times
                if (booking.BookingDate.Date < DateTime.Today || 
                    (booking.BookingDate.Date == DateTime.Today && booking.TimeSlot < DateTime.Now.TimeOfDay))
                {
                    return BadRequest("Bokningstiden kan inte vara i det förflutna.");
                }

                // Verify slot is still available
                var isBooked = await _context.Bookings.AnyAsync(b => 
                    b.BookingDate.Date == booking.BookingDate.Date && 
                    b.TimeSlot == booking.TimeSlot);

                if (isBooked)
                {
                    return BadRequest("This time slot is no longer available.");
                }

                // Verify slot is not blocked
                var isBlocked = await _context.BlockedSlots.AnyAsync(b => 
                    b.Date.Date == booking.BookingDate.Date && 
                    (b.TimeSlot == null || b.TimeSlot == booking.TimeSlot));

                if (isBlocked)
                {
                    return BadRequest("Denna tid är inte längre tillgänglig för bokning.");
                }

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                try 
                {
                   string messageBody = $"Hi {booking.CustomerName}, your car wash is confirmed for {booking.BookingDate:MMM dd} at {booking.TimeSlot:hh\\:mm}. See you then!";
                   await _smsService.SendSmsAsync(booking.PhoneNumber!, messageBody);
                }
                catch (Exception ex)
                {
                   Console.WriteLine($"Failed to send SMS: {ex.Message}");
                }

                try
                {
                    string subject = "Bokningsbekräftelse - Årstabilvård";
                    string emailBody = $@"
                        <div style='font-family: Arial, sans-serif; color: #333;'>
                            <h2>Tack {System.Net.WebUtility.HtmlEncode(booking.CustomerName)} för din bokning!</h2>
                            <p>Din bokning är nu bekräftad. Här är detaljerna:</p>
                            <ul>
                                <li><strong>Datum:</strong> {booking.BookingDate:yyyy-MM-dd}</li>
                                <li><strong>Tid:</strong> {booking.TimeSlot:hh\:mm}</li>
                                <li><strong>Tjänst:</strong> {System.Net.WebUtility.HtmlEncode(selectedService.Name)}</li>
                                <li><strong>Bilnummer:</strong> {System.Net.WebUtility.HtmlEncode(booking.CarPlateNumber)}</li>
                            </ul>
                            <p>Vi ses snart!</p>
                            <p style='margin-top: 20px; font-weight: bold;'>Vid ändring eller avbokning, vänligen kontakta oss på 📞 0737099970</p>
                            <hr />
                            <p style='font-size: 0.8em; color: #777;'>Årstabilvård - Din expert på bilvård</p>
                        </div>";
                    
                    await _emailService.SendEmailAsync(booking.Email!, subject, emailBody);
                }
                catch (Exception ex)
                {
                    HttpContext.RequestServices.GetRequiredService<ILogger<BookingController>>()
                        .LogError(ex, "Booking {BookingId} was saved but its confirmation email failed.", booking.Id);
                }

                return Ok(new { success = true, message = "Booking confirmed!" });
            }

            return BadRequest("Invalid booking data.");
        }
    }
}
