using Notification.Domain.Entities;

namespace Notification.Domain.Service;

public interface IMailSender
{
    void Send(Mail mail);
}
