namespace Doctor.Domain.Repositories.Contracts.ServiceDay;

public interface IServiceDayReadOnly
{
    Task<IEnumerable<Entities.ServiceDay>> RecoverByDoctorIdAsync(Guid doctorId);
    Task<IEnumerable<Entities.ServiceDay>> RecoverByDoctorIdAndDaysAsync(Guid doctorId, List<string> days);
}
