using Consultation.Domain.ModelServices;

namespace Consultation.Domain.Services;

public interface IUserServiceApi
{
    Task<Result<UserResult>> RecoverByEmailAsync(string email);
    Task<Result<UserResult>> RecoverByIdAsync(Guid clientId);
}
