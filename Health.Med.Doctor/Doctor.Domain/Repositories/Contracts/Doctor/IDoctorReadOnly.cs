namespace Doctor.Domain.Repositories.Contracts.Doctor;

public interface IDoctorReadOnly
{
    Task<Entities.Doctor> RecoverByEmailAsync(string email);
    Task<Entities.Doctor> RecoverByCRAsync(string cr);
    Task<IEnumerable<Entities.Doctor>> RecoverAllAsync(int page, int pageSize);
    Task<Entities.Doctor> RecoverByCRPasswordAsync(string cr, string password);
    Task<Entities.Doctor> RecoverByIdAsync(Guid id);
}
