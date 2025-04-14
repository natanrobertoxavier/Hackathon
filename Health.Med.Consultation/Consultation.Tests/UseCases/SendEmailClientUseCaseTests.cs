using Consultation.Application.DTO;
using Consultation.Application.Services.LoggedClientService;
using Consultation.Application.Settings;
using Consultation.Application.UseCase.SendEmailClient;
using Consultation.Communication.Request;
using Consultation.Domain.Entities.Enum;
using Consultation.Domain.Messages.DomainEvents;
using Consultation.Domain.ModelServices;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Serilog;

namespace Consultation.Tests.UseCases;

public class SendEmailClientUseCaseTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<IOptions<TemplateSettings>> _mockOptions;
    private readonly Mock<ILoggedClient> _mockLoggedClient;
    private readonly Mock<ILogger> _mockLogger;
    private SendEmailClientUseCase _useCase;

    public SendEmailClientUseCaseTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockOptions = new Mock<IOptions<TemplateSettings>>();
        _mockLoggedClient = new Mock<ILoggedClient>();
        _mockLogger = new Mock<ILogger>();
    }

    [Fact]
    public async Task SendEmailClientAsync_ShouldReturnSuccess_WhenEmailIsSent()
    {
        // Arrange
        var request = new RequestRegisterConsultation
        {
            DoctorId = Guid.NewGuid(),
            ConsultationDate = DateTime.UtcNow.AddHours(1)
        };

        var doctor = new DoctorResult
        {
            DoctorId = Guid.NewGuid(),
            Name = "Dr. John Doe",
            SpecialtyDoctor = new ResponseSpecialtyDoctor { Description = "Cardiology" }
        };

        var loggedUser = new LoggedUserDto(Guid.NewGuid(), "John Smith", "john.smith@example.com");

        _mockLoggedClient.Setup(x => x.GetLoggedClient()).Returns(loggedUser);

        _mockMediator
            .Setup(x => x.Publish(It.IsAny<SendEmailClientEvent>(), default))
            .Returns(Task.CompletedTask);

        GetInstanceUseCase("Consultation.Application.EmailTemplates");

        // Act
        var result = await _useCase.SendEmailSchedulingConsultationClientAsync(request, doctor, TemplateEmailEnum.ConsultationSchedulingClientEmail);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Email enviado com sucesso", result.Data.Message);
        _mockMediator.Verify(x => x.Publish(It.IsAny<SendEmailClientEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task SendEmailClientAsync_ShouldReturnFailure_WhenTemplateNotFound()
    {
        // Arrange
        var request = new RequestRegisterConsultation
        {
            DoctorId = Guid.NewGuid(),
            ConsultationDate = DateTime.UtcNow.AddHours(1)
        };

        var doctor = new DoctorResult
        {
            DoctorId = Guid.NewGuid(),
            Name = "Dr. John Doe",
            SpecialtyDoctor = new ResponseSpecialtyDoctor { Description = "Cardiology" }
        };

        var loggedUser = new LoggedUserDto(Guid.NewGuid(), "John Smith", "john.smith@example.com");

        _mockLoggedClient.Setup(x => x.GetLoggedClient()).Returns(loggedUser);

        GetInstanceUseCase("InvalidPath");

        // Act
        var result = await _useCase.SendEmailSchedulingConsultationClientAsync(request, doctor, TemplateEmailEnum.ConsultationSchedulingClientEmail);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Algo deu errado", result.Errors[0]);
    }

    [Fact]
    public async Task SendEmailClientAsync_ShouldReturnFailure_WhenExceptionOccurs()
    {
        // Arrange
        var request = new RequestRegisterConsultation
        {
            DoctorId = Guid.NewGuid(),
            ConsultationDate = DateTime.UtcNow.AddHours(1)
        };

        var doctor = new DoctorResult
        {
            DoctorId = Guid.NewGuid(),
            Name = "Dr. John Doe",
            SpecialtyDoctor = new ResponseSpecialtyDoctor { Description = "Cardiology" }
        };

        var loggedUser = new LoggedUserDto(Guid.NewGuid(), "John Smith", "john.smith@example.com");

        _mockLoggedClient.Setup(x => x.GetLoggedClient()).Returns(loggedUser);

        _mockMediator
            .Setup(x => x.Publish(It.IsAny<SendEmailClientEvent>(), default))
            .Throws(new Exception("Unexpected error"));

        GetInstanceUseCase("Consultation.Application.EmailTemplates");

        // Act
        var result = await _useCase.SendEmailSchedulingConsultationClientAsync(request, doctor, TemplateEmailEnum.ConsultationSchedulingClientEmail);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Algo deu errado", result.Errors[0]);
    }

    private void GetInstanceUseCase(string path)
    {
        _mockOptions.Setup(o => o.Value).Returns(new TemplateSettings()
        {
            ClientSettings = new ClientSettings()
            {
                PathTemplateClient = path,
                Subject = "Consulta Agendada"
            }
        });

        _useCase = new SendEmailClientUseCase(
            _mockMediator.Object,
            _mockOptions.Object,
            _mockLoggedClient.Object,
            _mockLogger.Object
        );
    }
}
