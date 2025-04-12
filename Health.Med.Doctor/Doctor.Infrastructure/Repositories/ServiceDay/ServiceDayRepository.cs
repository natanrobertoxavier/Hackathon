using Doctor.Domain.Entities;
using Doctor.Domain.Repositories.Contracts.ServiceDay;
using System.Linq;

namespace Doctor.Infrastructure.Repositories.ServiceDay;

public class ServiceDayRepository(HealthMedContext context) : IServiceDayWriteOnly, IServiceDayReadOnly
{
    private readonly HealthMedContext _context = context;

    public async Task AddAsync(Domain.Entities.ServiceDay serviceDay) =>
        await _context.AddAsync(serviceDay);
    public async Task AddAsync(IEnumerable<Domain.Entities.ServiceDay> serviceDays) =>
        await _context.AddRangeAsync(serviceDays);

    public void Update(IEnumerable<Domain.Entities.ServiceDay> serviceDays)
    {
        //var doctorId = serviceDays.FirstOrDefault().DoctorId;
        //var days = serviceDays.Select(x => x.Day).ToList();

        //var serviceDaysDB = _context.ServiceDays
        //    .Where(x => x.DoctorId == doctorId && days.Contains(x.Day))
        //    .ToList();

        //foreach (var day in serviceDaysDB)
        //{
        //    day.
        //}
    }

    public void Remove(Guid doctorId, IEnumerable<string> daysToRemove)
    {
        var serviceDays = _context.ServiceDays
            .Where(x => x.DoctorId == doctorId && daysToRemove.Contains(x.Day))
            .ToList();

        if (serviceDays.Any())
            _context.ServiceDays.RemoveRange(serviceDays);
    }
}
