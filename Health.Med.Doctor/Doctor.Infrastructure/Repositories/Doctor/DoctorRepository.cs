using Doctor.Domain.Repositories.Contracts;

namespace Doctor.Infrastructure.Repositories.Doctor;

public class DoctorRepository : IDoctorReadOnly, IDoctorWriteOnly
{
    public Task AddAsync(Domain.Entities.Doctor doctor)
    {
        throw new NotImplementedException();
    }
}
