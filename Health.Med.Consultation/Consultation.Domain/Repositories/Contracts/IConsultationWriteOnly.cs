namespace Consultation.Domain.Repositories.Contracts;

public interface IConsultationWriteOnly
{
    Task AddAsync(Entities.Consultation consultation);
}
