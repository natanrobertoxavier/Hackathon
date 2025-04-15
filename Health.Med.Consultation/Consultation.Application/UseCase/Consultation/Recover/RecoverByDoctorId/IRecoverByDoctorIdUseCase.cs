using Consultation.Communication.Response;

namespace Consultation.Application.UseCase.Consultation.Recover.RecoverByDoctorId;

public interface IRecoverByDoctorIdUseCase
{
    Task<Result<IEnumerable<ResponseConsultation>>> RecoverByDoctorIdAsync(Guid doctorId);
}
