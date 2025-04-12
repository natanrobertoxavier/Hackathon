using Doctor.Communication.Filter;
using System.Text.Json.Serialization;

namespace Doctor.Communication.Request;

public class RequestServiceDay(List<ServiceDay> serviceDays)
{
    public List<ServiceDay> ServiceDays { get; set; } = serviceDays;
}

public class ServiceDay
{
    [JsonConverter(typeof(DayOfWeekConverter))]
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}