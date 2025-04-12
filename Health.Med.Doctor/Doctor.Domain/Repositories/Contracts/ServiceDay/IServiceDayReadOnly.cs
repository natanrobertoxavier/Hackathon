namespace Doctor.Domain.Repositories.Contracts.ServiceDay;

public interface IServiceDayReadOnly
{
    Task<IEnumerable<Entities.ServiceDay>> GetByDoctorIdAsync(Guid doctorId);
    Task<IEnumerable<Entities.ServiceDay>> GetByDoctorIdAndDaysAsync(Guid doctorId, List<string> days);
}
