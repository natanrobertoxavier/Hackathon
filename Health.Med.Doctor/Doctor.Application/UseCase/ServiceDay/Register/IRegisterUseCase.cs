using Doctor.Communication.Request;
using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.ServiceDay.Register;

public interface IRegisterUseCase
{
    Task<Result<MessageResult>> RegisterServiceDayAsync(RequestServiceDay request);
}
