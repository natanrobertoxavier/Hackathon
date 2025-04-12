using Doctor.Communication.Filter;
using System.Text.Json.Serialization;

namespace Doctor.Communication.Request;

public class RequestDeleteServiceDay
{
    public IEnumerable<DaysToRemove> Days { get; set; }
}

public class DaysToRemove
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DayOfWeek Day { get; set; }
}
