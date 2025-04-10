using Moq;
using Serilog;
using User.Application.UseCase.Recover.RecoverByEmail;
using User.Domain.Repositories.Contracts;

namespace User.Tests.UseCases;

public class RecoverByEmailUseCaseTests
{
    private readonly Mock<IUserReadOnly> _userReadOnlyMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly RecoverByEmailUseCase _useCase;

    public RecoverByEmailUseCaseTests()
    {
        _userReadOnlyMock = new Mock<IUserReadOnly>();
        _loggerMock = new Mock<ILogger>();

        _useCase = new RecoverByEmailUseCase(
            _userReadOnlyMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task RecoverByEmailAsync_ShouldReturnSuccess_WhenUserIsFound()
    {
        // Arrange
        var email = "any@email.com";
        var user = new Domain.Entities.User(Guid.NewGuid(), DateTime.UtcNow, "User 1", "email1@example.com", "password1");

        _userReadOnlyMock
            .Setup(x => x.RecoverByEmailAsync(email))
            .ReturnsAsync(user);

        // Act
        var result = await _useCase.RecoverByEmailAsync(email);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(user.Name, result.Data.Name);
        Assert.Equal(user.Email, result.Data.Email);
        _loggerMock.Verify(x => x.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RecoverByEmailAsync_ShouldReturnSuccessWithNull_WhenUserIsNotFound()
    {
        // Arrange
        var email = "ay@email.com";

        _userReadOnlyMock
            .Setup(x => x.RecoverByEmailAsync(email))
            .ReturnsAsync(new Domain.Entities.User());

        // Act
        var result = await _useCase.RecoverByEmailAsync(email);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.Data);
        _loggerMock.Verify(x => x.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RecoverByEmailAsync_ShouldReturnFailure_WhenExceptionOccurs()
    {
        // Arrange
        var email = "any@email.com";

        _userReadOnlyMock
            .Setup(x => x.RecoverByEmailAsync(email))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _useCase.RecoverByEmailAsync(email);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Algo deu errado: Unexpected error", result.Errors);
        _loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }
}

