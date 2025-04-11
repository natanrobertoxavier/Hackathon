using Client.Application.UseCase.Recover.RecoverAll;
using Client.Domain.Repositories.Contracts;
using Moq;
using Serilog;

namespace Client.Tests.UseCases;

public class RecoverAllUseCaseTests
{
    private readonly Mock<IClientReadOnly> _clientReadOnlyMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly RecoverAllUseCase _useCase;

    public RecoverAllUseCaseTests()
    {
        _clientReadOnlyMock = new Mock<IClientReadOnly>();
        _loggerMock = new Mock<ILogger>();

        _useCase = new RecoverAllUseCase(
            _clientReadOnlyMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task RecoverAllAsync_ShouldReturnSuccess_WhenClientsAreFound()
    {
        // Arrange
        var encryptedPassword = GetEncryptedPassword();
        var clients = new List<Domain.Entities.Client>
        {
            new Domain.Entities.Client(Guid.NewGuid(), DateTime.UtcNow, "Client Name 1", "Client 1", "email@example.com", "12345", encryptedPassword),
            new Domain.Entities.Client(Guid.NewGuid(), DateTime.UtcNow, "Client Name 2", "Client 2", "email@example.com", "12345", encryptedPassword)
        };

        _clientReadOnlyMock
            .Setup(x => x.RecoverAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(clients);

        // Act
        var result = await _useCase.RecoverAllAsync(1, 5);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(clients.Count, result.Data.Count());
        _loggerMock.Verify(x => x.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RecoverAllAsync_ShouldReturnSuccessWithNull_WhenNoClientsAreFound()
    {
        // Arrange
        _clientReadOnlyMock
            .Setup(x => x.RecoverAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Domain.Entities.Client>());

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
        _clientReadOnlyMock
            .Setup(x => x.RecoverAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _useCase.RecoverAllAsync(1, 5);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Algo deu errado: Unexpected error", result.Errors);
        _loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }

    private static string GetEncryptedPassword() =>
       "207c5533da953c64419d3e7662e290f9d3b41745d3eb5dbab255a3002053d0a8c0a31f43f4e0db01a55f55b5ea694dd44c211178120b81dcf48ee55bc367c4fe";
}
