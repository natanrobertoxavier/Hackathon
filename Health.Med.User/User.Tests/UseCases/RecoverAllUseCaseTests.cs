using Moq;
using Serilog;
using User.Application.UseCase.Recover.RecoverAll;
using User.Domain.Repositories.Contracts;

namespace User.Tests.UseCases;

public class RecoverAllUseCaseTests
{
    private readonly Mock<IUserReadOnly> _userReadOnlyMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly RecoverAllUseCase _useCase;

    public RecoverAllUseCaseTests()
    {
        _userReadOnlyMock = new Mock<IUserReadOnly>();
        _loggerMock = new Mock<ILogger>();

        _useCase = new RecoverAllUseCase(
            _userReadOnlyMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task RecoverAllAsync_ShouldReturnSuccess_WhenUsersAreFound()
    {
        // Arrange
        var users = new List<Domain.Entities.User>
        {
            new Domain.Entities.User(Guid.NewGuid(), DateTime.UtcNow, "User 1", "email1@example.com", "password1"),
            new Domain.Entities.User(Guid.NewGuid(), DateTime.UtcNow, "User 2", "email2@example.com", "password2")
        };

        _userReadOnlyMock
            .Setup(x => x.RecoverAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(users);

        // Act
        var result = await _useCase.RecoverAllAsync(1, 5);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(users.Count, result.Data.Count());
        _loggerMock.Verify(x => x.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RecoverAllAsync_ShouldReturnSuccessWithNull_WhenNoUsersAreFound()
    {
        // Arrange
        _userReadOnlyMock
            .Setup(x => x.RecoverAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Domain.Entities.User>());

        // Act
        var result = await _useCase.RecoverAllAsync(1, 5);

        // Assert
        Assert.True(result.Success);
        Assert.True(!result.Data.Any());
        _loggerMock.Verify(x => x.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RecoverAllAsync_ShouldReturnFailure_WhenExceptionOccurs()
    {
        // Arrange
        _userReadOnlyMock
            .Setup(x => x.RecoverAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _useCase.RecoverAllAsync(1, 5);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Algo deu errado: Unexpected error", result.Errors);
        _loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }
}
