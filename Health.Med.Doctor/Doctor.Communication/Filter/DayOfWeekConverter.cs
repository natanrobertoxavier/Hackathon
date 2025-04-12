using System.Text.Json;
using System.Text.Json.Serialization;

namespace Doctor.Communication.Filter;

public class DayOfWeekConverter : JsonConverter<DayOfWeek>
{
    public override DayOfWeek Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dayString = reader.GetString();

        if (Enum.TryParse<DayOfWeek>(dayString, ignoreCase: true, out var day))
        {
            return day;
        }

        throw new JsonException($"Valor inválido para DayOfWeek: {dayString}");
    }

    public override void Write(Utf8JsonWriter writer, DayOfWeek value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
