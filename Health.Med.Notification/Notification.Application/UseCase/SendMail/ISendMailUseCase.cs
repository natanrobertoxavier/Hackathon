using Notification.Communication.Request;
using Notification.Communication.Response;

namespace Notification.Application.UseCase.SendMail;
public interface ISendMailUseCase
{
    Task<Result<MessageResult>> SendMailAsync(RequestSendMail request);
}
