using Doctor.Domain.ModelServices;

namespace Doctor.Domain.Services;

public interface IClientServiceApi
{
    Task<Result<ClientBasicInformationResult>> RecoverConsultationByDoctorIdAsync(string userEmail);
}
