using Doctor.Domain.Repositories.Contracts.Specialty;
using Microsoft.EntityFrameworkCore;

namespace Doctor.Infrastructure.Repositories.Specialty;

public class SpecialtyRepository(HealthMedContext context) : ISpecialtyReadOnly, ISpecialtyWriteOnly
{
    private readonly HealthMedContext _context = context;

    public async Task AddAsync(Domain.Entities.Specialty specialty) =>
        await _context.AddAsync(specialty);

    public void Update(Domain.Entities.Specialty specialty) =>
        _context.Specialties.Update(specialty);

    public async Task<Domain.Entities.Specialty> RecoverByIdAsync(Guid id) =>
        await _context.Specialties
        .FirstOrDefaultAsync(d => d.Id.Equals(id)) ?? new Domain.Entities.Specialty();

    public async Task<Domain.Entities.Specialty> RecoverByStandardDescriptionAsync(string standardizedDescription) =>
        await _context.Specialties
        .FirstOrDefaultAsync(d => d.StandardDescription.Equals(standardizedDescription)) ?? new Domain.Entities.Specialty();

    public async Task<IEnumerable<Domain.Entities.Specialty>> RecoverAllAsync(int skip, int pageSize) =>
        await _context.Specialties
        .AsNoTracking()
        .Skip(skip)
        .Take(pageSize)
        .ToListAsync();
}
