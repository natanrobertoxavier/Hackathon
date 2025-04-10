using Doctor.Application.UseCase.Doctor.Register;
using Doctor.Communication.Request;
using Doctor.Domain.Repositories;
using Doctor.Domain.Repositories.Contracts.Doctor;
using Microsoft.AspNetCore.Http;
using Moq;
using Serilog;
using TokenService.Manager.Controller;

namespace Doctor.Tests.UseCases.Doctor;

public class RegisterUseCaseTests
{
    private readonly Mock<IDoctorReadOnly> _doctorReadOnlyMock;
    private readonly Mock<IDoctorWriteOnly> _doctorWriteOnlyMock;
    private readonly Mock<IWorkUnit> _workUnitMock;
    private readonly Mock<PasswordEncryptor> _passwordEncryptorMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly RegisterUseCase _useCase;

    public RegisterUseCaseTests()
    {
        _doctorReadOnlyMock = new Mock<IDoctorReadOnly>();
        _doctorWriteOnlyMock = new Mock<IDoctorWriteOnly>();
        _workUnitMock = new Mock<IWorkUnit>();
        _passwordEncryptorMock = new Mock<PasswordEncryptor>("any-string");
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _loggerMock = new Mock<ILogger>();

        _useCase = new RegisterUseCase(
            _doctorReadOnlyMock.Object,
            _doctorWriteOnlyMock.Object,
            _workUnitMock.Object,
            _passwordEncryptorMock.Object,
            _httpContextAccessorMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task RegisterDoctorAsync_ShouldReturnSuccess_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var request = new RequestRegisterDoctor("Doctor Name", "email@example.com", "12345", "password", Guid.NewGuid());
        var userId = Guid.NewGuid();

        _httpContextAccessorMock
            .Setup(x => x.HttpContext.Items["AuthenticatedUser"])
            .Returns(new Domain.ModelServices.UserResult { Id = userId });

        _doctorReadOnlyMock
            .Setup(x => x.RecoverByEmailAsync(request.Email))
            .ReturnsAsync(new Domain.Entities.Doctor());

        _doctorReadOnlyMock
            .Setup(x => x.RecoverByCRAsync(request.CR))
            .ReturnsAsync(new Domain.Entities.Doctor());

        // Act
        var result = await _useCase.RegisterDoctorAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Cadastro realizado com sucesso", result.Data.Message);

        _doctorWriteOnlyMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Doctor>()), Times.Once);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterDoctorAsync_ShouldReturnValidationError_WhenEmailIsAlreadyRegistered()
    {
        // Arrange
        var request = new RequestRegisterDoctor("Doctor Name", "email@example.com", "12345", "password", Guid.NewGuid());
        var existingDoctor = new Domain.Entities.Doctor(Guid.NewGuid(), DateTime.UtcNow, "Doctor Name", request.Email, "12345", "password", Guid.NewGuid());

        _doctorReadOnlyMock
            .Setup(x => x.RecoverByEmailAsync(request.Email))
            .ReturnsAsync(existingDoctor);

        // Act
        var result = await _useCase.RegisterDoctorAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Email já cadastrado", result.Errors);

        _doctorWriteOnlyMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Doctor>()), Times.Never);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task RegisterDoctorAsync_ShouldReturnValidationError_WhenCRIsAlreadyRegistered()
    {
        // Arrange
        var request = new RequestRegisterDoctor("Doctor Name", "email@example.com", "12345", "password", Guid.NewGuid());
        var existingDoctor = new Domain.Entities.Doctor(Guid.NewGuid(), DateTime.UtcNow, "Doctor Name", "otheremail@example.com", request.CR, "password", Guid.NewGuid());

        _doctorReadOnlyMock
            .Setup(x => x.RecoverByEmailAsync(request.Email))
            .ReturnsAsync(new Domain.Entities.Doctor());

        _doctorReadOnlyMock
            .Setup(x => x.RecoverByCRAsync(request.CR))
            .ReturnsAsync(existingDoctor);

        // Act
        var result = await _useCase.RegisterDoctorAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Cadastro no órgão regional já cadastrado", result.Errors);

        _doctorWriteOnlyMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Doctor>()), Times.Never);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task RegisterDoctorAsync_ShouldReturnFailure_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        var request = new RequestRegisterDoctor("Doctor Name", "email@example.com", "12345", "password", Guid.NewGuid());

        _doctorReadOnlyMock
            .Setup(x => x.RecoverByEmailAsync(request.Email))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _useCase.RegisterDoctorAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Algo deu errado: Unexpected error", result.Errors);

        _doctorWriteOnlyMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Doctor>()), Times.Never);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}
