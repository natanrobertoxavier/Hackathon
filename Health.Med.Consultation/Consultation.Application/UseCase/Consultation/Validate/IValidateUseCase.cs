
using Consultation.Communication.Response;

namespace Consultation.Application.UseCase.Consultation.Validate;

public interface IValidateUseCase
{
    Task<Result<DateTime>> ValidateConsultationIdAsync(Guid consultationId, Guid doctorId);
    Task<Result<DateTime>> ValidateConsultationClientIdAsync(Guid consultationId, Guid doctorId);
}
