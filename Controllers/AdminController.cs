using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarWashStation.Data;
using CarWashStation.Models;

namespace CarWashStation.Controllers
{
    [Authorize] // This whole controller requires login
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, DateTime? filterDate)
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            var query = _context.Bookings.Include(b => b.Service).AsQueryable();

            // Stats calculation (unfiltered)
            var statsQuery = _context.Bookings.Include(b => b.Service);
            
            var viewModel = new AdminDashboardViewModel
            {
                TotalBookingsCount = await statsQuery.CountAsync(),
                NewThisWeekCount = await statsQuery.CountAsync(b => b.BookingDate >= startOfWeek && b.BookingDate <= today),
                TodayCount = await statsQuery.CountAsync(b => b.BookingDate == today),
                RemainingTodayCount = await statsQuery.CountAsync(b => b.BookingDate == today && b.TimeSlot > DateTime.Now.TimeOfDay),
                MonthlyRevenue = await statsQuery
                    .Where(b => b.BookingDate >= startOfMonth)
                    .SumAsync(b => b.Service != null ? b.Service.Price : 0),
                TotalCustomersCount = await statsQuery
                    .Select(b => b.PhoneNumber)
                    .Distinct()
                    .CountAsync(),
                CorporateCustomersCount = await statsQuery
                    .Where(b => !string.IsNullOrEmpty(b.CompanyName))
                    .Select(b => b.CompanyName)
                    .Distinct()
                    .CountAsync()
            };

            // Apply filters for the table
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(b => 
                    (b.CompanyName != null && b.CompanyName.Contains(searchString)) || 
                    (b.CarPlateNumber != null && b.CarPlateNumber.Contains(searchString)) ||
                    (b.CustomerName != null && b.CustomerName.Contains(searchString)));
            }

            if (filterDate.HasValue)
            {
                query = query.Where(b => b.BookingDate.Date == filterDate.Value.Date);
            }

            viewModel.RecentBookings = await query
                .OrderByDescending(b => b.BookingDate)
                .ThenBy(b => b.TimeSlot)
                .ToListAsync();

            viewModel.CurrentFilter = searchString;
            viewModel.CurrentDateFilter = filterDate?.ToString("yyyy-MM-dd");

            return View(viewModel);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (booking == null) return NotFound();

            ViewBag.Services = await _context.Services.ToListAsync();
            return View(booking);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerName,Email,PhoneNumber,CarPlateNumber,ServiceId,BookingDate,TimeSlot,CompanyName,Notes")] Booking booking)
        {
            if (id != booking.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Services = await _context.Services.ToListAsync();
            return View(booking);
        }

        // POST: Admin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/ScheduleControl
        public async Task<IActionResult> ScheduleControl()
        {
            var blockedSlots = await _context.BlockedSlots
                .Where(b => b.Date >= DateTime.Today)
                .OrderBy(b => b.Date)
                .ThenBy(b => b.TimeSlot)
                .ToListAsync();
            return View(blockedSlots);
        }

        // POST: Admin/BlockSlot
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockSlot(DateTime date, string? timeSlot, string? reason)
        {
            TimeSpan? ts = null;
            if (!string.IsNullOrEmpty(timeSlot))
            {
                if (TimeSpan.TryParse(timeSlot, out var parsedTs))
                {
                    ts = parsedTs;
                }
            }

            var blockedSlot = new BlockedSlot
            {
                Date = date.Date,
                TimeSlot = ts,
                Reason = reason
            };

            _context.BlockedSlots.Add(blockedSlot);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ScheduleControl));
        }

        // POST: Admin/UnblockSlot
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnblockSlot(int id)
        {
            var blockedSlot = await _context.BlockedSlots.FindAsync(id);
            if (blockedSlot != null)
            {
                _context.BlockedSlots.Remove(blockedSlot);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ScheduleControl));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}
