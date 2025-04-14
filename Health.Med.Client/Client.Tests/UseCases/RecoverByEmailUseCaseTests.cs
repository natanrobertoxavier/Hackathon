using Client.Application.UseCase.Recover.RecoverByCPF;
using Client.Application.UseCase.Recover.RecoverByEmail;
using Client.Domain.Repositories.Contracts;
using Moq;
using Serilog;

namespace Client.Tests.UseCases;

public class RecoverByEmailUseCaseTests
{
    private readonly Mock<IClientReadOnly> _clientReadOnlyMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly RecoverByIdUseCase _useCase;

    public RecoverByEmailUseCaseTests()
    {
        _clientReadOnlyMock = new Mock<IClientReadOnly>();
        _loggerMock = new Mock<ILogger>();

        _useCase = new RecoverByIdUseCase(
            _clientReadOnlyMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task RecoverByEmailAsync_ShouldReturnSuccess_WhenClientIsFound()
    {
        // Arrange
        var encryptedPassword = GetEncryptedPassword();
        var email = "any@email.com";
        var client = new Domain.Entities.Client(Guid.NewGuid(), DateTime.UtcNow, "Client Name 1", "Client 1", "email@example.com", "12345", encryptedPassword);

        _clientReadOnlyMock
            .Setup(x => x.RecoverByEmailAsync(email))
            .ReturnsAsync(client);

        // Act
        var result = await _useCase.RecoverByEmailAsync(email);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(client.Name, result.Data.Name);
        Assert.Equal(client.Email, result.Data.Email);
        _loggerMock.Verify(x => x.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RecoverByEmailAsync_ShouldReturnSuccessWithNull_WhenClientIsNotFound()
    {
        // Arrange
        var email = "any@email.com";

        _clientReadOnlyMock
            .Setup(x => x.RecoverByEmailAsync(email))
            .ReturnsAsync(new Domain.Entities.Client());

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
        // Arrange;
        var email = "any@email.com";

        _clientReadOnlyMock
            .Setup(x => x.RecoverByEmailAsync(email))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _useCase.RecoverByEmailAsync(email);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Algo deu errado: Unexpected error", result.Errors);
        _loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }

    private static string GetEncryptedPassword() =>
       "207c5533da953c64419d3e7662e290f9d3b41745d3eb5dbab255a3002053d0a8c0a31f43f4e0db01a55f55b5ea694dd44c211178120b81dcf48ee55bc367c4fe";
}

