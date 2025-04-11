using Client.Application.Services;
using Client.Application.UseCase.ChangePassword;
using Client.Communication.Request;
using Client.Domain.Repositories;
using Client.Domain.Repositories.Contracts;
using Moq;
using Serilog;
using TokenService.Manager.Controller;

namespace Client.Tests.UseCases;

public class ChangePasswordUseCaseTests
{
    private readonly Mock<IWorkUnit> _workUnitMock;
    private readonly Mock<PasswordEncryptor> _passwordEncryptorMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly Mock<ILoggedClient> _loggedUserMock;
    private readonly Mock<IClientReadOnly> _clientReadOnlyMock;
    private readonly Mock<IClientWriteOnly> _clientWriteOnlyMock;
    private readonly ChangePasswordUseCase _useCase;

    public ChangePasswordUseCaseTests()
    {
        _workUnitMock = new Mock<IWorkUnit>();
        _passwordEncryptorMock = new Mock<PasswordEncryptor>("any-string");
        _loggerMock = new Mock<ILogger>();
        _loggedUserMock = new Mock<ILoggedClient>();
        _clientReadOnlyMock = new Mock<IClientReadOnly>();
        _clientWriteOnlyMock = new Mock<IClientWriteOnly>();

        _useCase = new ChangePasswordUseCase(
            _workUnitMock.Object,
            _passwordEncryptorMock.Object,
            _loggerMock.Object,
            _loggedUserMock.Object,
            _clientReadOnlyMock.Object,
            _clientWriteOnlyMock.Object
        );
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnSuccess_WhenPasswordIsChangedSuccessfully()
    {
        // Arrange
        var request = new RequestChangePassword("oldPassword", "newPassword");
        var loggedDoctor = new Domain.Entities.Client(Guid.NewGuid(), DateTime.UtcNow, "Client Name", "Client", "email@example.com", "12345", GetOldPassword());

        _loggedUserMock
            .Setup(x => x.GetLoggedClientAsync())
            .ReturnsAsync(loggedDoctor);

        _clientReadOnlyMock
            .Setup(x => x.RecoverByIdAsync(loggedDoctor.Id))
            .ReturnsAsync(loggedDoctor);

        // Act
        var result = await _useCase.ChangePasswordAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Senha atualizada com sucesso", result.Data.Message);

        _clientWriteOnlyMock.Verify(x => x.Update(It.IsAny<Domain.Entities.Client>()), Times.Once);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnValidationError_WhenCurrentPasswordIsInvalid()
    {
        // Arrange
        var request = new RequestChangePassword("wrongPassword", "newPassword");
        var loggedDoctor = new Domain.Entities.Client(Guid.NewGuid(), DateTime.UtcNow, "Client Name", "Client", "email@example.com", "12345", "encryptedOldPassword");

        _loggedUserMock
            .Setup(x => x.GetLoggedClientAsync())
            .ReturnsAsync(loggedDoctor);

        _clientReadOnlyMock
            .Setup(x => x.RecoverByIdAsync(loggedDoctor.Id))
            .ReturnsAsync(loggedDoctor);

        // Act
        var result = await _useCase.ChangePasswordAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Senha atual inválida", result.Errors);

        _clientWriteOnlyMock.Verify(x => x.Update(It.IsAny<Domain.Entities.Client>()), Times.Never);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnFailure_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        var request = new RequestChangePassword("oldPassword", "newPassword");

        _loggedUserMock
            .Setup(x => x.GetLoggedClientAsync())
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _useCase.ChangePasswordAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Algo deu errado: Unexpected error", result.Errors);

        _clientWriteOnlyMock.Verify(x => x.Update(It.IsAny<Domain.Entities.Client>()), Times.Never);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    private static string GetOldPassword() =>
       "207c5533da953c64419d3e7662e290f9d3b41745d3eb5dbab255a3002053d0a8c0a31f43f4e0db01a55f55b5ea694dd44c211178120b81dcf48ee55bc367c4fe";
}
