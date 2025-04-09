using Consultation.Domain.ModelServices;

namespace Consultation.Domain.Services;

public interface IDoctorServiceApi
{
    Task<Result<DoctorResult>> RecoverByIdAsync(Guid cr);
}