namespace Doctor.Domain.Repositories.Contracts.ServiceDay;

public interface IServiceDayWriteOnly
{
    Task AddAsync(Entities.ServiceDay serviceDay);
    Task AddAsync(IEnumerable<Entities.ServiceDay> serviceDays);
    void Remove(Guid doctorId, IEnumerable<string> daysToRemove);
}
