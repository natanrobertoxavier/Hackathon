namespace Consultation.Application.Extensions;

public static class UsefulExtensions
{
    public static DateTime TrimMilliseconds(this DateTime dateTime) =>
        new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
}
