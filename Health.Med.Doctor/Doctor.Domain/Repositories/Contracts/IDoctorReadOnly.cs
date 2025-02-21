namespace Doctor.Domain.Repositories.Contracts;

public interface IDoctorReadOnly
{
    Task<Entities.Doctor> ThereIsWithEmailAsync(string email);
    Task<Entities.Doctor> ThereIsWithCR(string cr);
}
