namespace Doctor.Domain.Entities;

public class ServiceDay : BaseEntity
{

    public ServiceDay(
        Guid id,
        DateTime registrationDate,
        Guid doctorId,
        string day, 
        TimeSpan startTime, 
        TimeSpan endTime) : base(id, registrationDate)
    {
        DoctorId = doctorId;
        Day = day;
        StartTime = startTime;
        EndTime = endTime;
    }
    public ServiceDay(
        Guid doctorId,
        string day, 
        TimeSpan startTime, 
        TimeSpan endTime)
    {
        DoctorId = doctorId;
        Day = day;
        StartTime = startTime;
        EndTime = endTime;
    }

    public ServiceDay()
    {
    }

    public Guid DoctorId { get; set; }
    public string Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public virtual Doctor Doctor { get; set; }
}
