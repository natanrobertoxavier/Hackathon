using Client.Application.UseCase.Register;
using Client.Communication.Request;
using Client.Domain.Repositories;
using Client.Domain.Repositories.Contracts;
using Moq;
using Serilog;
using TokenService.Manager.Controller;

namespace Client.Tests.UseCases;

public class RegisterUseCaseTests
{
    private readonly Mock<IClientReadOnly> _clientReadOnlyMock;
    private readonly Mock<IClientWriteOnly> _clientWriteOnlyMock;
    private readonly Mock<IWorkUnit> _workUnitMock;
    private readonly Mock<PasswordEncryptor> _passwordEncryptorMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly RegisterUseCase _useCase;

    public RegisterUseCaseTests()
    {
        _clientReadOnlyMock = new Mock<IClientReadOnly>();
        _clientWriteOnlyMock = new Mock<IClientWriteOnly>();
        _workUnitMock = new Mock<IWorkUnit>();
        _passwordEncryptorMock = new Mock<PasswordEncryptor>("any-string");
        _loggerMock = new Mock<ILogger>();

        _useCase = new RegisterUseCase(
            _clientReadOnlyMock.Object,
            _clientWriteOnlyMock.Object,
            _workUnitMock.Object,
            _passwordEncryptorMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task RegisterClientAsync_ShouldReturnSuccess_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var request = new RequestRegisterClient(
            "John Doe",
            "John",
            "john.doe@example.com",
            "12345678900",
            "password123");

        _clientReadOnlyMock
            .Setup(x => x.RecoverByEmailAsync(request.Email))
            .ReturnsAsync(new Domain.Entities.Client());

        _clientReadOnlyMock
            .Setup(x => x.RecoverByCPFAsync(request.CPF))
            .ReturnsAsync(new Domain.Entities.Client());

        // Act
        var result = await _useCase.RegisterClientAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Cadastro realizado com sucesso", result.Data.Message);
        _clientWriteOnlyMock.Verify(x => x.AddAsync(It.IsAny<Client.Domain.Entities.Client>()), Times.Once);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Once);
        _loggerMock.Verify(x => x.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RegisterClientAsync_ShouldReturnFailure_WhenEmailIsAlreadyRegistered()
    {
        // Arrange
        var request = new RequestRegisterClient("John Doe",
            "John",
            "john.doe@example.com",
            "12345678900",
            "password123");

        var existingClient = new Domain.Entities.Client(
            Guid.NewGuid(),
            DateTime.UtcNow,
            "Existing User",
            "Existing",
            request.Email,
            "12345678900",
            "encryptedPassword"
        );

        _clientReadOnlyMock
            .Setup(x => x.RecoverByEmailAsync(request.Email))
            .ReturnsAsync(existingClient);

        // Act
        var result = await _useCase.RegisterClientAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Email já cadastrado", result.Errors);
        _clientWriteOnlyMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Client>()), Times.Never);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Never);
        _loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task RegisterClientAsync_ShouldReturnFailure_WhenCPFIsAlreadyRegistered()
    {
        // Arrange
        var request = new RequestRegisterClient("John Doe",
            "John",
            "john.doe@example.com",
            "12345678900",
            "password123");

        var existingClient = new Domain.Entities.Client(
            Guid.NewGuid(),
            DateTime.UtcNow,
            "Existing User",
            "Existing",
            "existing.email@example.com",
            request.CPF,
            "encryptedPassword"
        );

        _clientReadOnlyMock
            .Setup(x => x.RecoverByEmailAsync(request.Email))
            .ReturnsAsync(new Domain.Entities.Client());

        _clientReadOnlyMock
            .Setup(x => x.RecoverByCPFAsync(request.CPF))
            .ReturnsAsync(existingClient);

        // Act
        var result = await _useCase.RegisterClientAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("CPF já cadastrado", result.Errors);
        _clientWriteOnlyMock.Verify(x => x.AddAsync(It.IsAny<Client.Domain.Entities.Client>()), Times.Never);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Never);
        _loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task RegisterClientAsync_ShouldReturnFailure_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        var request = new RequestRegisterClient("John Doe",
            "John",
            "john.doe@example.com",
            "12345678900",
            "password123");

        _clientReadOnlyMock
            .Setup(x => x.RecoverByEmailAsync(request.Email))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _useCase.RegisterClientAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Algo deu errado: Unexpected error", result.Errors);
        _clientWriteOnlyMock.Verify(x => x.AddAsync(It.IsAny<Client.Domain.Entities.Client>()), Times.Never);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Never);
        _loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }
}
