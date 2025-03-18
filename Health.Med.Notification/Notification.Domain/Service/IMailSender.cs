using Notification.Domain.Entities;

namespace Notification.Domain.Service;

public interface IMailSender
{
    Task SendAsync(Mail mail);
}
