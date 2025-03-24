using System.ComponentModel;
using System.Reflection;

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
}
