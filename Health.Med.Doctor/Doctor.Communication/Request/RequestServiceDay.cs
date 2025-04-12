using Doctor.Communication.Filter;
using System.Text.Json.Serialization;

namespace Doctor.Communication.Request;

public class RequestServiceDay
{
    public RequestServiceDay(List<ServiceDay> serviceDays)
    {
        ServiceDays = serviceDays;
    }

    public RequestServiceDay()
    {
    }

    public List<ServiceDay> ServiceDays { get; set; }

}

public class ServiceDay
{
    public ServiceDay(
        DayOfWeek day, 
        TimeSpan startTime, 
        TimeSpan endTime)
    {
        Day = day;
        StartTime = startTime;
        EndTime = endTime;
    }

    public ServiceDay()
    {
    }

    [JsonConverter(typeof(DayOfWeekConverter))]
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}