using Doctor.Application.Services.Doctor;
using Doctor.Application.UseCase.Doctor.ChangePassword;
using Doctor.Communication.Request;
using Doctor.Domain.Repositories;
using Doctor.Domain.Repositories.Contracts.Doctor;
using Moq;
using Serilog;
using TokenService.Manager.Controller;

namespace Doctor.Tests.UseCases.Doctor;

public class ChangePasswordUseCaseTests
{
    private readonly Mock<IWorkUnit> _workUnitMock;
    private readonly Mock<PasswordEncryptor> _passwordEncryptorMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly Mock<ILoggedDoctor> _loggedDoctorMock;
    private readonly Mock<IDoctorReadOnly> _doctorReadOnlyMock;
    private readonly Mock<IDoctorWriteOnly> _doctorWriteOnlyMock;
    private readonly ChangePasswordUseCase _useCase;

    public ChangePasswordUseCaseTests()
    {
        _workUnitMock = new Mock<IWorkUnit>();
        _passwordEncryptorMock = new Mock<PasswordEncryptor>("any-string");
        _loggerMock = new Mock<ILogger>();
        _loggedDoctorMock = new Mock<ILoggedDoctor>();
        _doctorReadOnlyMock = new Mock<IDoctorReadOnly>();
        _doctorWriteOnlyMock = new Mock<IDoctorWriteOnly>();

        _useCase = new ChangePasswordUseCase(
            _workUnitMock.Object,
            _passwordEncryptorMock.Object,
            _loggerMock.Object,
            _loggedDoctorMock.Object,
            _doctorReadOnlyMock.Object,
            _doctorWriteOnlyMock.Object
        );
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnSuccess_WhenPasswordIsChangedSuccessfully()
    {
        // Arrange
        var request = new RequestChangePassword("oldPassword", "newPassword");
        var loggedDoctor = new Domain.Entities.Doctor(Guid.NewGuid(), DateTime.UtcNow, "Doctor Name", "Name", "email@example.com", "12345", GetOldPassword(), Guid.NewGuid());

        _loggedDoctorMock
            .Setup(x => x.GetLoggedDoctorAsync())
            .ReturnsAsync(loggedDoctor);

        _doctorReadOnlyMock
            .Setup(x => x.RecoverByIdAsync(loggedDoctor.Id))
            .ReturnsAsync(loggedDoctor);

        // Act
        var result = await _useCase.ChangePasswordAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Senha atualizada com sucesso", result.Data.Message);

        _doctorWriteOnlyMock.Verify(x => x.Update(It.IsAny<Domain.Entities.Doctor>()), Times.Once);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnValidationError_WhenCurrentPasswordIsInvalid()
    {
        // Arrange
        var request = new RequestChangePassword("wrongPassword", "newPassword");
        var loggedDoctor = new Domain.Entities.Doctor(Guid.NewGuid(), DateTime.UtcNow, "Doctor Name", "Name", "email@example.com", "12345", "encryptedOldPassword", Guid.NewGuid());

        _loggedDoctorMock
            .Setup(x => x.GetLoggedDoctorAsync())
            .ReturnsAsync(loggedDoctor);

        _doctorReadOnlyMock
            .Setup(x => x.RecoverByIdAsync(loggedDoctor.Id))
            .ReturnsAsync(loggedDoctor);

        // Act
        var result = await _useCase.ChangePasswordAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Senha atual inválida", result.Errors);

        _doctorWriteOnlyMock.Verify(x => x.Update(It.IsAny<Domain.Entities.Doctor>()), Times.Never);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnFailure_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        var request = new RequestChangePassword("oldPassword", "newPassword");

        _loggedDoctorMock
            .Setup(x => x.GetLoggedDoctorAsync())
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _useCase.ChangePasswordAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Algo deu errado: Unexpected error", result.Errors);

        _doctorWriteOnlyMock.Verify(x => x.Update(It.IsAny<Domain.Entities.Doctor>()), Times.Never);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    private static string GetOldPassword() =>
       "207c5533da953c64419d3e7662e290f9d3b41745d3eb5dbab255a3002053d0a8c0a31f43f4e0db01a55f55b5ea694dd44c211178120b81dcf48ee55bc367c4fe";
}
