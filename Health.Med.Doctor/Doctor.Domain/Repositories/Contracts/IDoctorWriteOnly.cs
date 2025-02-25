
namespace Doctor.Domain.Repositories.Contracts;

public interface IDoctorWriteOnly
{
    Task AddAsync(Entities.Doctor doctor);
    void Update(Entities.Doctor doctor);
}
