using Doctor.Domain.Entities;
using Doctor.Domain.Repositories.Contracts.ServiceDay;
using Microsoft.EntityFrameworkCore;

namespace Doctor.Infrastructure.Repositories.ServiceDay;

public class ServiceDayRepository(HealthMedContext context) : IServiceDayWriteOnly, IServiceDayReadOnly
{
    private readonly HealthMedContext _context = context;

    public async Task AddAsync(Domain.Entities.ServiceDay serviceDay) =>
        await _context.AddAsync(serviceDay);
    public async Task AddAsync(IEnumerable<Domain.Entities.ServiceDay> serviceDays) =>
        await _context.AddRangeAsync(serviceDays);

    public void Remove(Guid doctorId, IEnumerable<string> daysToRemove)
    {
        var serviceDays = _context.ServiceDays
            .Where(x => x.DoctorId == doctorId && daysToRemove.Contains(x.Day))
            .ToList();

        if (serviceDays.Any())
            _context.ServiceDays.RemoveRange(serviceDays);
    }

    public async Task<IEnumerable<Domain.Entities.ServiceDay>> RecoverByDoctorIdAsync(Guid doctorId) =>
        await _context.ServiceDays
            .Where(x => x.DoctorId == doctorId)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Domain.Entities.ServiceDay>> RecoverByDoctorIdAndDaysAsync(Guid doctorId, List<string> days) =>
        await _context.ServiceDays
            .Where(x => x.DoctorId == doctorId && days.Contains(x.Day))
            .ToListAsync();
}
