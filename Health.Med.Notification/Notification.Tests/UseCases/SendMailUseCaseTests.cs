using Moq;
using Notification.Application.UseCase.SendMail;
using Notification.Communication.Request;
using Notification.Domain.Entities;
using Notification.Domain.Service;
using Serilog;

namespace Notification.Tests.UseCases;

public class SendMailUseCaseTests
{
    private readonly Mock<IMailSender> _mockMailSender;
    private readonly Mock<ILogger> _mockLogger;
    private readonly SendMailUseCase _useCase;

    public SendMailUseCaseTests()
    {
        _mockMailSender = new Mock<IMailSender>();
        _mockLogger = new Mock<ILogger>();
        _useCase = new SendMailUseCase(_mockMailSender.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task SendAsync_ShouldReturnSuccess_WhenMailIsSentSuccessfully()
    {
        // Arrange
        var request = new RequestSendMail(
            recipients: new List<string> { "test@example.com" },
            copyRecipients: new List<string>(),
            hiddenRecipients: new List<string>(),
            subject: "Test Subject",
            body: "Test Body"
        );

        _mockMailSender
            .Setup(x => x.SendAsync(It.IsAny<Mail>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.SendAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("E-mail enviado com sucesso", result.Data.Message);
        _mockMailSender.Verify(x => x.SendAsync(It.IsAny<Mail>()), Times.Once);
        _mockLogger.Verify(x => x.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task SendAsync_ShouldReturnValidationError_WhenValidationFails()
    {
        // Arrange
        var request = new RequestSendMail(
            recipients: new List<string>(),
            copyRecipients: new List<string>(),
            hiddenRecipients: new List<string>(),
            subject: "Test Subject",
            body: "Test Body"
        );

        // Act
        var result = await _useCase.SendAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Destinatário em branco", result.Errors);
        _mockLogger.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SendAsync_ShouldReturnFailure_WhenMailSenderThrowsException()
    {
        // Arrange
        var request = new RequestSendMail(
            recipients: new List<string> { "test@example.com" },
            copyRecipients: new List<string>(),
            hiddenRecipients: new List<string>(),
            subject: "Test Subject",
            body: "Test Body"
        );

        _mockMailSender
            .Setup(x => x.SendAsync(It.IsAny<Mail>()))
            .ThrowsAsync(new Exception("SMTP error"));

        // Act
        var result = await _useCase.SendAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Algo deu errado: SMTP error", result.Errors);
        _mockLogger.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }
}
