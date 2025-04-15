using Doctor.Domain.ModelServices;

namespace Doctor.Domain.Services;

public interface IConsultationServiceApi
{
    Task<Result<IEnumerable<ConsultationResult>>> RecoverConsultationByDoctorIdAsync(Guid doctorId);
}
