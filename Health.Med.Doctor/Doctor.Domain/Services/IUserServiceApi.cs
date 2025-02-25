using Doctor.Domain.ModelServices;

namespace Doctor.Domain.Services;

public interface IUserServiceApi
{
    Task<Result<UserResult>> RecoverByEmailAsync(string email);
}
