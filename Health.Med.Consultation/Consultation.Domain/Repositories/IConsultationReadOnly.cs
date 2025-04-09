namespace Consultation.Domain.Repositories;

public interface IConsultationReadOnly
{
    Task<bool> ThereIsConsultationForDoctor(Guid id, DateTime consultationDate);
    Task<bool> ThereIsConsultationForClient(Guid id, DateTime consultationDate);
}
