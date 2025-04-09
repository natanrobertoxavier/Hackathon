using Notification.Api.QueueModels;
using Notification.Communication.Request;

namespace Notification.Api.Mapping;

public static class MailMapping
{

    public static RequestSendMail MessageToRequest(this SendMailModel request)
    {
        return new RequestSendMail(
            request.Recipients,
            request.CopyRecipients,
            request.HiddenRecipients,
            request.Subject,
            request.Body,
            request.IsHtml
        );
    }
}
