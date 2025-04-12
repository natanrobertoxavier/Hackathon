namespace Doctor.Communication.Response;

public class ResponseServiceDay
{
    public ResponseServiceDay(
        string day, 
        TimeSpan startTime, 
        TimeSpan endTime)
    {
        Day = day;
        StartTime = startTime;
        EndTime = endTime;
    }

    public ResponseServiceDay()
    {
    }

    public string Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
