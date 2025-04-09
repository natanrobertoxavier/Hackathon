namespace Doctor.Domain.Repositories.Contracts.Specialty;

public interface ISpecialtyWriteOnly
{
    Task AddAsync(Entities.Specialty specialty);
    void Update(Entities.Specialty specialty);
}