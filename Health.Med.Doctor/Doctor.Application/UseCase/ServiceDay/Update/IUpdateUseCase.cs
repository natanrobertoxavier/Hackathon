using Doctor.Communication.Request;
using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.ServiceDay.Update;

public interface IUpdateUseCase
{
    Task<Result<MessageResult>> UpdateServiceDayAsync(RequestServiceDay request);
}
