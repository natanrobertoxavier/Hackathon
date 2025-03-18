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

    public async Task SendAsync(Mail mail)
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
        await smtp.SendMailAsync(mailMessage);
    }

    private static void AddEmailsToMailMessage(MailMessage mailMessage, Mail mail)
    {
        mail.Recipients?.ForEach(mailMessage.To.Add);
        mail.CopyRecipients?.Where(copy => !string.IsNullOrEmpty(copy)).ToList().ForEach(mailMessage.CC.Add);
        mail.HiddenRecipients?.Where(hidden => !string.IsNullOrEmpty(hidden)).ToList().ForEach(mailMessage.Bcc.Add);
    }
}
