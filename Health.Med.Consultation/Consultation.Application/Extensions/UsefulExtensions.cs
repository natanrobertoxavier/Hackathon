using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Consultation.Application.Extensions;

public static class UsefulExtensions
{
    public static DateTime TrimMilliseconds(this DateTime dateTime) =>
        new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);

    public static string GetDescription(this Enum value)
    {
        FieldInfo? field = value.GetType().GetField(value.ToString());
        DescriptionAttribute? attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attribute == null ? value.ToString() : attribute.Description;
    }
    public static DateTime ToSPDateZone(this DateTime utcDate)
    {
        if (utcDate.Kind != DateTimeKind.Utc)
            throw new ArgumentException("A data precisa estar em UTC.", nameof(utcDate));

        TimeZoneInfo saoPauloTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "E. South America Standard Time"
                : "America/Sao_Paulo"
        );

        return TimeZoneInfo.ConvertTimeFromUtc(utcDate, saoPauloTimeZone);
    }
}
