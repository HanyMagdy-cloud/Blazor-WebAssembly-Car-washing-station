using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CarWashStation.Models;

// A booking date is a calendar day at the shop, not an instant in time.
public sealed class BookingDateJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Preserve the written day even for older clients sending midnight with an offset.
        return reader.GetDateTimeOffset().Date;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
    }
}
