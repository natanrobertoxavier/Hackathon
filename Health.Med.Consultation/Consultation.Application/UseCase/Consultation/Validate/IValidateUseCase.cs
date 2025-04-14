
using Consultation.Communication.Response;

namespace Consultation.Application.UseCase.Consultation.Validate;

public interface IValidateUseCase
{
    Task<Result<bool>> ValidateConsultationIdAsync(Guid consultationId);
}
