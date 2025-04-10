using Moq;
using Serilog;
using TokenService.Manager.Controller;
using User.Application.Services;
using User.Application.UseCase.ChangePassword;
using User.Communication.Request;
using User.Domain.Repositories;
using User.Domain.Repositories.Contracts;

namespace User.Tests.UseCases;

public class ChangePasswordUseCaseTests
{
    private readonly Mock<IWorkUnit> _workUnitMock;
    private readonly Mock<PasswordEncryptor> _passwordEncryptorMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly Mock<ILoggedUser> _loggedUserMock;
    private readonly Mock<IUserReadOnly> _userReadOnlyMock;
    private readonly Mock<IUserWriteOnly> _userWriteOnlyMock;
    private readonly ChangePasswordUseCase _useCase;

    public ChangePasswordUseCaseTests()
    {
        _workUnitMock = new Mock<IWorkUnit>();
        _passwordEncryptorMock = new Mock<PasswordEncryptor>("any-string");
        _loggerMock = new Mock<ILogger>();
        _loggedUserMock = new Mock<ILoggedUser>();
        _userReadOnlyMock = new Mock<IUserReadOnly>();
        _userWriteOnlyMock = new Mock<IUserWriteOnly>();

        _useCase = new ChangePasswordUseCase(
            _workUnitMock.Object,
            _passwordEncryptorMock.Object,
            _loggerMock.Object,
            _loggedUserMock.Object,
            _userReadOnlyMock.Object,
            _userWriteOnlyMock.Object
        );
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnSuccess_WhenPasswordIsChangedSuccessfully()
    {
        // Arrange
        var request = new RequestChangePassword("oldPassword", "newPassword");
        var loggedUser = new Domain.Entities.User(Guid.NewGuid(), DateTime.UtcNow, "User Name", "email@example.com", GetOldPassword());

        _loggedUserMock
            .Setup(x => x.GetLoggedUserAsync())
            .ReturnsAsync(loggedUser);

        _userReadOnlyMock
            .Setup(x => x.RecoverByIdAsync(loggedUser.Id))
            .ReturnsAsync(loggedUser);

        // Act
        var result = await _useCase.ChangePasswordAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Senha atualizada com sucesso", result.Data.Message);

        _userWriteOnlyMock.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Once);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnValidationError_WhenCurrentPasswordIsInvalid()
    {
        // Arrange
        var request = new RequestChangePassword("wrongPassword", "newPassword");
        var loggedUser = new Domain.Entities.User(Guid.NewGuid(), DateTime.UtcNow, "User Name", "email@example.com", GetOldPassword());

        _loggedUserMock
            .Setup(x => x.GetLoggedUserAsync())
            .ReturnsAsync(loggedUser);

        _userReadOnlyMock
            .Setup(x => x.RecoverByIdAsync(loggedUser.Id))
            .ReturnsAsync(loggedUser);

        // Act
        var result = await _useCase.ChangePasswordAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Senha atual inválida", result.Errors);

        _userWriteOnlyMock.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Never);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnFailure_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        var request = new RequestChangePassword("oldPassword", "newPassword");

        _loggedUserMock
            .Setup(x => x.GetLoggedUserAsync())
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _useCase.ChangePasswordAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Algo deu errado: Unexpected error", result.Errors);

        _userWriteOnlyMock.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Never);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    private static string GetOldPassword() =>
       "207c5533da953c64419d3e7662e290f9d3b41745d3eb5dbab255a3002053d0a8c0a31f43f4e0db01a55f55b5ea694dd44c211178120b81dcf48ee55bc367c4fe";
}
