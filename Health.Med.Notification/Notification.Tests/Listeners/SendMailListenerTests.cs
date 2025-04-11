using Microsoft.Extensions.DependencyInjection;
using Moq;
using Notification.Api.Listeners;
using Notification.Api.QueueModels;
using Notification.Application.UseCase.SendMail;
using Notification.Communication.Request;
using Notification.Communication.Response;
using RabbitMQ.Client;
using Serilog;

namespace Notification.Tests.Listeners;

public class SendMailListenerTests
{
    private readonly Mock<IConnectionFactory> _mockConnectionFactory;
    private readonly Mock<ILogger> _mockLogger;
    private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
    private readonly Mock<IServiceScope> _mockScope;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<ISendMailUseCase> _mockSendMailUseCase;
    private readonly SendMailListener _listener;

    public SendMailListenerTests()
    {
        _mockConnectionFactory = new Mock<IConnectionFactory>();
        _mockLogger = new Mock<ILogger>();
        _mockScopeFactory = new Mock<IServiceScopeFactory>();
        _mockScope = new Mock<IServiceScope>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockSendMailUseCase = new Mock<ISendMailUseCase>();

        _mockScopeFactory.Setup(f => f.CreateScope()).Returns(_mockScope.Object);
        _mockScope.Setup(s => s.ServiceProvider).Returns(_mockServiceProvider.Object);
        _mockServiceProvider.Setup(p => p.GetService(typeof(ISendMailUseCase)))
            .Returns(_mockSendMailUseCase.Object);

        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IModel>();

        mockConnection.Setup(c => c.CreateModel()).Returns(mockChannel.Object);

        _mockConnectionFactory
            .Setup(factory => factory.CreateConnection())
            .Returns(mockConnection.Object);


        _listener = new SendMailListener(
            _mockConnectionFactory.Object,
            _mockLogger.Object,
            _mockScopeFactory.Object
        );
    }

    [Fact]
    public async Task ProcessMessageAsync_ShouldLogInformation_WhenMessageIsProcessedSuccessfully()
    {
        // Arrange
        var message = new SendMailModel
        {
            Recipients = new List<string> { "recipient@example.com" },
            Subject = "Test Subject",
            Body = "Test Body",
            IsHtml = true
        };

        var result = new Result<MessageResult>();
        result.Succeeded(new MessageResult("E-mail enviado com sucesso"));

        _mockSendMailUseCase
            .Setup(u => u.SendAsync(It.IsAny<RequestSendMail>()))
            .ReturnsAsync(result);

        // Act
        var method = typeof(SendMailListener).GetMethod("ProcessMessageAsync",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        await (Task)method.Invoke(_listener, new object[] { message });

        // Assert
        _mockLogger.Verify(
            l => l.Information(It.Is<string>(s => s.Contains("Início do processamento da mensagem"))),
            Times.Once
        );
        _mockLogger.Verify(
            l => l.Information(It.Is<string>(s => s.Contains("Fim do processamento da mensagem"))),
            Times.Once
        );
    }

    [Fact]
    public async Task ProcessMessageAsync_ShouldLogError_WhenSendMailFails()
    {
        // Arrange
        var message = new SendMailModel
        {
            Recipients = new List<string> { "recipient@example.com" },
            Subject = "Test Subject",
            Body = "Test Body",
            IsHtml = true
        };

        var result = new Result<MessageResult>();
        result.Failure(new List<string>() { "Error sending email" });

        _mockSendMailUseCase
            .Setup(u => u.SendAsync(It.IsAny<RequestSendMail>()))
            .ReturnsAsync(result);

        // Act
        var method = typeof(SendMailListener).GetMethod("ProcessMessageAsync",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        await (Task)method.Invoke(_listener, new object[] { message });

        // Assert
        _mockLogger.Verify(
            l => l.Error(It.Is<string>(s => s.Contains("Ocorreu um erro ao enviar o e-mail"))),
            Times.Once
        );
    }

    [Fact]
    public async Task ProcessMessageAsync_ShouldHandleException_WhenAnErrorOccurs()
    {
        // Arrange
        var message = new SendMailModel
        {
            Recipients = new List<string> { "recipient@example.com" },
            Subject = "Test Subject",
            Body = "Test Body",
            IsHtml = true
        };

        _mockSendMailUseCase
            .Setup(u => u.SendAsync(It.IsAny<RequestSendMail>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var method = typeof(SendMailListener).GetMethod("ProcessMessageAsync",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        await (Task)method.Invoke(_listener, new object[] { message });

        // Assert
        _mockLogger.Verify(
            l => l.Error(It.Is<string>(s => s.Contains("Ocorreu um erro durante o processamento da mensagem"))),
            Times.Once
        );
    }
}
