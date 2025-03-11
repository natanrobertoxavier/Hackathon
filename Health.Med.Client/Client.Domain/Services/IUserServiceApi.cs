using Client.Domain.ModelServices;

namespace Client.Domain.Services;

public interface IUserServiceApi
{
    Task<Result<UserResult>> RecoverByEmailAsync(string email);
}
