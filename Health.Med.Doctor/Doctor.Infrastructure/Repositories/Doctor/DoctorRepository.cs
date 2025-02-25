using Doctor.Domain.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Doctor.Infrastructure.Repositories.Doctor;

public class DoctorRepository(HealthMedContext context) : IDoctorReadOnly, IDoctorWriteOnly
{
    private readonly HealthMedContext _context = context;

    public async Task AddAsync(Domain.Entities.Doctor doctor) =>
        await _context.AddAsync(doctor);

    public void Update(Domain.Entities.Doctor doctor) =>
        _context.Doctors.Update(doctor);

    public async Task<Domain.Entities.Doctor> RecoverByEmailAsync(string email) =>
        await _context.Doctors
        .AsNoTracking()
        .FirstOrDefaultAsync(d => d.Email.Equals(email)) ??
        new Domain.Entities.Doctor();

    public async Task<Domain.Entities.Doctor> RecoverByCRAsync(string cr) =>
        await _context.Doctors
        .AsNoTracking()
        .FirstOrDefaultAsync(d => d.CR.Equals(cr)) ??
        new Domain.Entities.Doctor();

    public async Task<IEnumerable<Domain.Entities.Doctor>> RecoverAllAsync(int skip, int pageSize) =>
        await _context.Doctors
        .AsNoTracking()
        .Skip(skip)
        .Take(pageSize)
        .ToListAsync();

    public async Task<Domain.Entities.Doctor> RecoverByCRPasswordAsync(string cr, string password) =>
        await _context.Doctors
        .AsNoTracking()
        .FirstOrDefaultAsync(
            d => d.CR.Equals(cr) &&
            d.Password.Equals(password)
        ) ?? new Domain.Entities.Doctor();

    public async Task<Domain.Entities.Doctor> RecoverByIdAsync(Guid id) =>
        await _context.Doctors
        .FirstOrDefaultAsync(d => d.Id.Equals(id)) ?? new Domain.Entities.Doctor();
}
