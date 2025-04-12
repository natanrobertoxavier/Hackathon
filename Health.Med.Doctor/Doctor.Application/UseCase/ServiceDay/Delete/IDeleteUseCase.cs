using Doctor.Communication.Request;
using Doctor.Communication.Response;

namespace Doctor.Application.UseCase.ServiceDay.Delete;

public interface IDeleteUseCase
{
    Task<Result<MessageResult>> DeleteServiceDayAsync(RequestDeleteServiceDay request);
}
