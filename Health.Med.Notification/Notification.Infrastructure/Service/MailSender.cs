using Microsoft.Extensions.Options;
using Notification.Domain.Entities;
using Notification.Domain.Service;
using Notification.Infrastructure.Settings;
using System.Net;
using System.Net.Mail;

namespace Notification.Infrastructure.Service;

public class MailSender(
    IOptions<MailSettings> options) : IMailSender
{
    private readonly MailSettings _options = options.Value;

    public void Send(Mail mail)
    {
        using MailMessage mailMessage = new();

        mailMessage.From = new MailAddress(_options.From);
        mailMessage.Subject = mail.Subject;
        mailMessage.Body = mail.Body;
        mailMessage.IsBodyHtml = mail.IsHtml;
        mailMessage.Priority = MailPriority.Normal;

        AddEmailsToMailMessage(mailMessage, mail);

        using SmtpClient smtp = new(_options.SMTP, _options.Port);
        smtp.EnableSsl = true;
        smtp.Credentials = new NetworkCredential(_options.From, _options.Key);

        smtp.UseDefaultCredentials = false;
        smtp.Send(mailMessage);
    }

    private static void AddEmailsToMailMessage(MailMessage mailMessage, Mail mail)
    {
        foreach (var recipient in mail.Recipients)
            mailMessage.To.Add(recipient);

        if (mail.CopyRecipients?.Count > 0)
        {
            foreach (var copy in mail.CopyRecipients)
                if (!string.IsNullOrEmpty(copy))
                    mailMessage.CC.Add(copy);
        }

        if (mail.HiddenRecipients?.Count > 0)
        {
            foreach (var hidden in mail.HiddenRecipients)
                if (!string.IsNullOrEmpty(hidden))
                    mailMessage.Bcc.Add(hidden);
        }
    }
}
