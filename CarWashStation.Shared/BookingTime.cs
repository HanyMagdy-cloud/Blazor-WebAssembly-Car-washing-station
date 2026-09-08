namespace CarWashStation.Models;

public static class BookingTime
{
    private static readonly TimeZoneInfo ShopTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");

    public static DateTime Now => At(DateTimeOffset.UtcNow);

    public static DateTime At(DateTimeOffset instant) => TimeZoneInfo.ConvertTime(instant, ShopTimeZone).DateTime;
}
