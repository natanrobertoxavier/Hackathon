using Doctor.Communication.Filter;
using System.Text.Json.Serialization;

namespace Doctor.Communication.Request;

public class RequestDeleteServiceDay
{
    public RequestDeleteServiceDay(IEnumerable<DaysToRemove> days)
    {
        Days = days;
    }

    public RequestDeleteServiceDay()
    {
    }

    public IEnumerable<DaysToRemove> Days { get; set; }
}

public class DaysToRemove
{
    public DaysToRemove(DayOfWeek day)
    {
        Day = day;
    }

    public DaysToRemove()
    {
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DayOfWeek Day { get; set; }
}
