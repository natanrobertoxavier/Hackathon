using Consultation.Application.Services.LoggedClientService;
using Consultation.Application.UseCase.Consultation.Register;
using Consultation.Application.UseCase.SendEmailClient;
using Consultation.Application.UseCase.SendEmailDoctor;
using Consultation.Communication.Request;
using Consultation.Domain.ModelServices;
using Consultation.Domain.Repositories;
using Consultation.Domain.Repositories.Contracts;
using Consultation.Domain.Services;
using Moq;
using Serilog;
using System.Globalization;

namespace Consultation.Tests.UseCases;

public class RegisterUseCaseTests
{
    private readonly Mock<ILoggedClient> _mockLoggedClient;
    private readonly Mock<IConsultationReadOnly> _mockConsultationReadOnly;
    private readonly Mock<IConsultationWriteOnly> _mockConsultationWriteOnly;
    private readonly Mock<IDoctorServiceApi> _mockDoctorServiceApi;
    private readonly Mock<IWorkUnit> _mockWorkUnit;
    private readonly Mock<ISendEmailClientUseCase> _mockSendEmailClientUseCase;
    private readonly Mock<ISendEmailDoctorUseCase> _mockSendEmailDoctorUseCase;
    private readonly Mock<ILogger> _mockLogger;
    private readonly RegisterUseCase _useCase;

    public RegisterUseCaseTests()
    {
        _mockLoggedClient = new Mock<ILoggedClient>();
        _mockConsultationReadOnly = new Mock<IConsultationReadOnly>();
        _mockConsultationWriteOnly = new Mock<IConsultationWriteOnly>();
        _mockDoctorServiceApi = new Mock<IDoctorServiceApi>();
        _mockWorkUnit = new Mock<IWorkUnit>();
        _mockSendEmailClientUseCase = new Mock<ISendEmailClientUseCase>();
        _mockSendEmailDoctorUseCase = new Mock<ISendEmailDoctorUseCase>();
        _mockLogger = new Mock<ILogger>();

        _useCase = new RegisterUseCase(
            _mockLoggedClient.Object,
            _mockConsultationReadOnly.Object,
            _mockConsultationWriteOnly.Object,
            _mockDoctorServiceApi.Object,
            _mockWorkUnit.Object,
            _mockSendEmailClientUseCase.Object,
            _mockSendEmailDoctorUseCase.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task RegisterConsultationAsync_ShouldReturnSuccess_WhenAllValidationsPass()
    {
        // Arrange
        var request = new RequestRegisterConsultation
        {
            DoctorId = Guid.NewGuid(),
            ConsultationDate = DateTime.Parse("2026/04/16 10:00:00")
        };

        var loggedClientId = Guid.NewGuid();
        var serviceDays = new List<ResponseServiceDay>()
        {
            new ResponseServiceDay()
            {
                Day = CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.GetDayName(DayOfWeek.Thursday),
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(17, 0, 0)
            }
        };
        var doctorResult = new Domain.ModelServices.Result<DoctorResult>
        {
            Success = true,
            Data = new DoctorResult 
            {
                DoctorId = request.DoctorId, 
                ServiceDays = serviceDays
            }
        };

        _mockLoggedClient.Setup(x => x.GetLoggedClientId()).Returns(loggedClientId);
        _mockDoctorServiceApi.Setup(x => x.RecoverByIdAsync(request.DoctorId)).ReturnsAsync(doctorResult);
        _mockConsultationReadOnly.Setup(x => x.ThereIsConsultationForClient(loggedClientId, It.IsAny<DateTime>())).ReturnsAsync(false);
        _mockConsultationReadOnly.Setup(x => x.ThereIsConsultationForDoctor(request.DoctorId, It.IsAny<DateTime>())).ReturnsAsync(false);

        // Act
        var result = await _useCase.RegisterConsultationAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Cadastro realizado com sucesso", result.Data.Message);
        _mockConsultationWriteOnly.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Consultation>()), Times.Once);
        _mockWorkUnit.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterConsultationAsync_ShouldReturnFailure_WhenDoctorNotFound()
    {
        // Arrange
        var request = new RequestRegisterConsultation
        {
            DoctorId = Guid.NewGuid(),
            ConsultationDate = DateTime.Parse("2025/04/10 10:00:00.000")
        };

        var loggedClientId = Guid.NewGuid();
        var doctorResult = new Domain.ModelServices.Result<DoctorResult>
        {
            Success = false,
            Error = "Doctor not found"
        };

        _mockLoggedClient.Setup(x => x.GetLoggedClientId()).Returns(loggedClientId);
        _mockDoctorServiceApi.Setup(x => x.RecoverByIdAsync(request.DoctorId)).ReturnsAsync(doctorResult);

        // Act
        var result = await _useCase.RegisterConsultationAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Doctor not found", result.Errors);
        _mockConsultationWriteOnly.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Consultation>()), Times.Never);
        _mockWorkUnit.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task RegisterConsultationAsync_ShouldReturnFailure_WhenValidationFails()
    {
        // Arrange
        var request = new RequestRegisterConsultation
        {
            DoctorId = Guid.NewGuid(),
            ConsultationDate = DateTime.Parse("2025/04/10 10:12:00.000")
        };

        var loggedClientId = Guid.NewGuid();
        var doctorResult = new Domain.ModelServices.Result<DoctorResult>
        {
            Success = true,
            Data = new DoctorResult { DoctorId = request.DoctorId }
        };

        _mockLoggedClient.Setup(x => x.GetLoggedClientId()).Returns(loggedClientId);
        _mockDoctorServiceApi.Setup(x => x.RecoverByIdAsync(request.DoctorId)).ReturnsAsync(doctorResult);

        // Act
        var result = await _useCase.RegisterConsultationAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Horário inválido", result.Errors);
        _mockConsultationWriteOnly.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Consultation>()), Times.Never);
        _mockWorkUnit.Verify(x => x.CommitAsync(), Times.Never);
    }
}
