
namespace Consultation.Domain.Repositories.Contracts;

public interface IConsultationWriteOnly
{
    Task AddAsync(Entities.Consultation consultation);
    Task AcceptConsultationAsync(Guid consultationId, DateTime date);
    Task RefuseConsultationAsync(Guid consultationId, DateTime date);
    Task ClientCancelConsultationAsync(Guid consultationId, string reason, DateTime date);
}
