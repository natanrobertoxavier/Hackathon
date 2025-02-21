using Doctor.Domain.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Doctor.Infrastructure.Repositories.Doctor;

public class DoctorRepository(HealthMedContext context) : IDoctorReadOnly, IDoctorWriteOnly
{
    private readonly HealthMedContext _context = context;
    public async Task AddAsync(Domain.Entities.Doctor doctor) =>
        await _context.AddAsync(doctor);

    public async Task<Domain.Entities.Doctor> ThereIsWithEmailAsync(string email) =>
        await _context.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.Email.Equals(email)) ??
        new Domain.Entities.Doctor();

    public async Task<Domain.Entities.Doctor> ThereIsWithCR(string cr) =>
        await _context.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.CR.Equals(cr)) ??
        new Domain.Entities.Doctor();
}
