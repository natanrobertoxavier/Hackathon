using Microsoft.Extensions.Options;
using Moq;
using Notification.Domain.Entities;
using Notification.Infrastructure.Service;
using Notification.Infrastructure.Settings;
using System.Net.Mail;

namespace Notification.Tests.Services;

public class MailSenderTests
{
    private readonly Mock<IOptions<MailSettings>> _mockOptions;
    private readonly MailSender _mailSender;

    public MailSenderTests()
    {
        _mockOptions = new Mock<IOptions<MailSettings>>();
        _mockOptions.Setup(o => o.Value).Returns(new MailSettings
        {
            SMTP = "smtp.test.com",
            Port = 587,
            From = "test@example.com",
            Key = "password"
        });

        _mailSender = new MailSender(_mockOptions.Object);
    }

    [Fact]
    public async Task SendAsync_ShouldThrowException_WhenSMTPSettingsAreInvalid()
    {
        // Arrange
        _mockOptions.Setup(o => o.Value).Returns(new MailSettings
        {
            SMTP = "invalid.smtp.com",
            Port = 587,
            From = "test@example.com",
            Key = "password"
        });

        var mailSender = new MailSender(_mockOptions.Object);

        var mail = new Mail(
            recipients: new List<string> { "recipient@example.com" },
            copyRecipiens: new List<string>(),
            hiddenRecipients: new List<string>(),
            subject: "Test Subject",
            body: "Test Body",
            isHtml: true
        );

        // Act & Assert
        await Assert.ThrowsAsync<SmtpException>(() => mailSender.SendAsync(mail));
    }

    [Fact]
    public async Task SendAsync_ShouldThrowException_WhenRecipientsAreEmpty()
    {
        // Arrange
        var mail = new Mail(
            recipients: new List<string>(),
            copyRecipiens: new List<string>(),
            hiddenRecipients: new List<string>(),
            subject: "Test Subject",
            body: "Test Body",
            isHtml: true
        );

        // Act & Assert
        await Assert.ThrowsAsync<SmtpException>(() => _mailSender.SendAsync(mail));
    }
}
