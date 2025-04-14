using Client.Domain.ModelServices;

namespace Client.Domain.Services;

public interface IDoctorServiceApi
{
    Task<Result<DoctorResult>> RecoverByEmailAsync(string email);
}
