using Moq;
using Serilog;
using TokenService.Manager.Controller;
using User.Application.UseCase.Login;
using User.Communication.Request;
using User.Domain.Repositories.Contracts;

namespace User.Tests.UseCases;

public class LoginUseCaseTests
{
    private readonly Mock<IUserReadOnly> _userReadOnlyMock;
    private readonly Mock<PasswordEncryptor> _passwordEncryptorMock;
    private readonly Mock<TokenController> _tokenControllerMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly LoginUseCase _useCase;

    public LoginUseCaseTests()
    {
        _userReadOnlyMock = new Mock<IUserReadOnly>();
        _passwordEncryptorMock = new Mock<PasswordEncryptor>("any-string");
        _tokenControllerMock = new Mock<TokenController>(1000, "TWluaGFDaGF2ZVNlY3JldGExMjM=");
        _loggerMock = new Mock<ILogger>();

        _useCase = new LoginUseCase(
            _userReadOnlyMock.Object,
            _passwordEncryptorMock.Object,
            _tokenControllerMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenLoginIsSuccessful()
    {
        // Arrange
        var request = new RequestLoginUser("any@email.com", "password");
        var encryptedPassword = GetEncryptedPassword();
        var doctor = new Domain.Entities.User(Guid.NewGuid(), DateTime.UtcNow, "Doctor Name", "any@email.com", encryptedPassword);

        _userReadOnlyMock
            .Setup(x => x.RecoverByEmailPasswordAsync(request.Email, It.IsAny<string>()))
            .ReturnsAsync(doctor);

        // Act
        var result = await _useCase.LoginAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(doctor.Name, result.Data.Name);
        Assert.Equal(doctor.Email, result.Data.Email);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenDoctorNotFound()
    {
        // Arrange
        var request = new RequestLoginUser("any@email.com", "password");

        _userReadOnlyMock
            .Setup(x => x.RecoverByEmailPasswordAsync(request.Email, It.IsAny<string>()))
            .ReturnsAsync(new Domain.Entities.User());

        // Act
        var result = await _useCase.LoginAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("E-mail ou senha incorretos", result.Errors);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        var request = new RequestLoginUser("any@email.com", "password");

        _userReadOnlyMock
            .Setup(x => x.RecoverByEmailPasswordAsync(request.Email, It.IsAny<string>()))
            .Throws(new Exception("Unexpected error"));

        // Act
        var result = await _useCase.LoginAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Algo deu errado: Unexpected error", result.Errors);
    }

    private static string GetEncryptedPassword() =>
       "207c5533da953c64419d3e7662e290f9d3b41745d3eb5dbab255a3002053d0a8c0a31f43f4e0db01a55f55b5ea694dd44c211178120b81dcf48ee55bc367c4fe";
}
