using CarWashStation.Data;
using CarWashStation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarWashStation.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public sealed class AdminController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<ActionResult<AdminDashboardViewModel>> Dashboard(string? search = null, DateTime? date = null)
    {
        var today = DateTime.Today;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var query = context.Bookings.Include(b => b.Service).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(b => (b.CompanyName ?? "").Contains(search) ||
                                     (b.CarPlateNumber ?? "").Contains(search) ||
                                     (b.CustomerName ?? "").Contains(search));
        if (date.HasValue) query = query.Where(b => b.BookingDate.Date == date.Value.Date);

        return Ok(new AdminDashboardViewModel
        {
            TotalBookingsCount = await context.Bookings.CountAsync(),
            NewThisWeekCount = await context.Bookings.CountAsync(b => b.BookingDate >= startOfWeek && b.BookingDate <= today),
            TodayCount = await context.Bookings.CountAsync(b => b.BookingDate == today),
            RemainingTodayCount = await context.Bookings.CountAsync(b => b.BookingDate == today && b.TimeSlot > DateTime.Now.TimeOfDay),
            MonthlyRevenue = await context.Bookings.Where(b => b.BookingDate >= startOfMonth)
                .SumAsync(b => b.Service != null ? b.Service.Price : 0),
            TotalCustomersCount = await context.Bookings.Select(b => b.PhoneNumber).Distinct().CountAsync(),
            CorporateCustomersCount = await context.Bookings.Where(b => !string.IsNullOrEmpty(b.CompanyName))
                .Select(b => b.CompanyName).Distinct().CountAsync(),
            RecentBookings = await query.OrderByDescending(b => b.BookingDate).ThenBy(b => b.TimeSlot).ToListAsync(),
            CurrentFilter = search,
            CurrentDateFilter = date?.ToString("yyyy-MM-dd")
        });
    }

    [HttpGet("bookings/{id:int}")]
    public async Task<ActionResult<Booking>> GetBooking(int id) =>
        await context.Bookings.Include(b => b.Service).FirstOrDefaultAsync(b => b.Id == id) is { } booking
            ? Ok(booking) : NotFound();

    [HttpPut("bookings/{id:int}")]
    public async Task<IActionResult> UpdateBooking(int id, Booking booking)
    {
        if (id != booking.Id) return BadRequest();
        var existing = await context.Bookings.FindAsync(id);
        if (existing is null) return NotFound();
        if (!await context.Services.AnyAsync(service => service.Id == booking.ServiceId))
            return BadRequest("Den valda tjänsten finns inte.");

        existing.CustomerName = booking.CustomerName;
        existing.Email = booking.Email;
        existing.PhoneNumber = booking.PhoneNumber;
        existing.CarPlateNumber = booking.CarPlateNumber;
        existing.ServiceId = booking.ServiceId;
        existing.BookingDate = booking.BookingDate.Date;
        existing.TimeSlot = booking.TimeSlot;
        existing.CompanyName = booking.CompanyName;
        existing.Notes = booking.Notes;
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("bookings/{id:int}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await context.Bookings.FindAsync(id);
        if (booking is null) return NotFound();
        context.Bookings.Remove(booking);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("blocked-slots")]
    public async Task<IReadOnlyList<BlockedSlot>> BlockedSlots() =>
        await context.BlockedSlots.Where(b => b.Date >= DateTime.Today).OrderBy(b => b.Date).ThenBy(b => b.TimeSlot).ToListAsync();

    [HttpPost("blocked-slots")]
    public async Task<ActionResult<BlockedSlot>> BlockSlot(BlockSlotRequest request)
    {
        TimeSpan? time = string.IsNullOrWhiteSpace(request.TimeSlot) ? null : TimeSpan.Parse(request.TimeSlot);
        var slot = new BlockedSlot { Date = request.Date.Date, TimeSlot = time, Reason = request.Reason };
        context.BlockedSlots.Add(slot);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(BlockedSlots), slot);
    }

    [HttpDelete("blocked-slots/{id:int}")]
    public async Task<IActionResult> UnblockSlot(int id)
    {
        var slot = await context.BlockedSlots.FindAsync(id);
        if (slot is null) return NotFound();
        context.BlockedSlots.Remove(slot);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
