
using Consultation.Domain.ModelServices;

namespace Consultation.Domain.Services;

public interface IClientServiceApi
{
    Task<Result<ClientBasicInformationResult>> RecoverBasicInformationByEmailAsync(string userEmail);
}
