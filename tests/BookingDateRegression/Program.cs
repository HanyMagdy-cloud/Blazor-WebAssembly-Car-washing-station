using System.Text.Json;
using CarWashStation.Models;

static void Check(bool condition, string name)
{
    if (!condition) throw new InvalidOperationException(name);
    Console.WriteLine($"PASS: {name}");
}

var expected = new DateTime(2026, 9, 9);
var legacyValue = "2026-09-09T00:00:00+02:00";
var utcServerDate = JsonSerializer.Deserialize<DateTimeOffset>($"\"{legacyValue}\"").UtcDateTime;
Check(utcServerDate.Date == new DateTime(2026, 9, 8), "Reproduces old UTC server date shifting to September 8");

foreach (var input in new[] { "2026-09-09", "2026-09-09T00:00:00", legacyValue, "2026-09-09T00:00:00Z", "2026-09-09T00:00:00-07:00" })
{
    var booking = JsonSerializer.Deserialize<Booking>($$"""{"BookingDate":"{{input}}"}""")!;
    Check(booking.BookingDate == expected && booking.BookingDate.Kind == DateTimeKind.Unspecified, $"Preserves calendar date: {input}");
    var now = BookingTime.At(new DateTimeOffset(2026, 9, 8, 17, 0, 0, TimeSpan.Zero));
    Check(Enumerable.Range(0, 17).All(i => booking.BookingDate.AddHours(8).AddMinutes(30 * i) > now), "All tomorrow's working slots pass the past-time comparison");
}

foreach (var kind in new[] { DateTimeKind.Local, DateTimeKind.Utc, DateTimeKind.Unspecified })
{
    var json = JsonSerializer.Serialize(new Booking { BookingDate = DateTime.SpecifyKind(expected, kind) });
    Check(JsonDocument.Parse(json).RootElement.GetProperty("BookingDate").GetString() == "2026-09-09", $"Writes date without timezone for {kind}");
}

Check(BookingTime.At(DateTimeOffset.Parse("2026-09-08T22:30:00Z")) == new DateTime(2026, 9, 9, 0, 30, 0), "Stockholm summer midnight boundary");
Check(BookingTime.At(DateTimeOffset.Parse("2026-01-08T23:30:00Z")) == new DateTime(2026, 1, 9, 0, 30, 0), "Stockholm winter midnight boundary");
Check(BookingTime.At(DateTimeOffset.Parse("2026-03-29T01:00:00Z")) == new DateTime(2026, 3, 29, 3, 0, 0), "Stockholm daylight saving transition");
var sameDayNow = BookingTime.At(DateTimeOffset.Parse("2026-09-09T07:00:00Z"));
Check(expected.AddHours(8) < sameDayNow && expected.AddHours(10) > sameDayNow, "Past times remain rejected while later times remain valid");
