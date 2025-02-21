namespace Doctor.Domain.Repositories.Contracts;

public interface IDoctorReadOnly
{
    Task<Entities.Doctor> RecoverByEmailAsync(string email);
    Task<Entities.Doctor> RecoverByCRAsync(string cr);
    Task<IEnumerable<Entities.Doctor>> RecoverAllAsync(int page, int pageSize);
}
