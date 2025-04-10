using Doctor.Application.Services.User;
using Doctor.Application.UseCase.Specialty.Register;
using Doctor.Communication.Request;
using Doctor.Domain.Repositories;
using Doctor.Domain.Repositories.Contracts.Specialty;
using Moq;
using Serilog;

namespace Doctor.Tests.UseCases.Specialty;

public class RegisterUseCaseTests
{
    private readonly Mock<ISpecialtyReadOnly> _specialtyReadOnlyMock;
    private readonly Mock<ISpecialtyWriteOnly> _specialtyWriteOnlyMock;
    private readonly Mock<IWorkUnit> _workUnitMock;
    private readonly Mock<ILoggedUser> _loggedUserMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly RegisterUseCase _useCase;

    public RegisterUseCaseTests()
    {
        _specialtyReadOnlyMock = new Mock<ISpecialtyReadOnly>();
        _specialtyWriteOnlyMock = new Mock<ISpecialtyWriteOnly>();
        _workUnitMock = new Mock<IWorkUnit>();
        _loggedUserMock = new Mock<ILoggedUser>();
        _loggerMock = new Mock<ILogger>();

        _useCase = new RegisterUseCase(
            _specialtyReadOnlyMock.Object,
            _specialtyWriteOnlyMock.Object,
            _workUnitMock.Object,
            _loggedUserMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task RegisterSpecialtyAsync_ShouldReturnSuccess_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var request = new RequestRegisterSpecialty("Cardiology");
        var normalizedDescription = "CARDIOLOGY";
        var userId = Guid.NewGuid();

        _specialtyReadOnlyMock
            .Setup(x => x.RecoverByStandardDescriptionAsync(normalizedDescription))
            .ReturnsAsync(new Domain.Entities.Specialty());

        _loggedUserMock
            .Setup(x => x.GetLoggedUserId())
            .Returns(userId);

        // Act
        var result = await _useCase.RegisterSpecialtyAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Cadastro realizado com sucesso", result.Data.Message);

        _specialtyWriteOnlyMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Specialty>()), Times.Once);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterSpecialtyAsync_ShouldReturnValidationError_WhenSpecialtyAlreadyExists()
    {
        // Arrange
        var request = new RequestRegisterSpecialty("Cardiology");
        var normalizedDescription = "CARDIOLOGY";
        var existingSpecialty = new Domain.Entities.Specialty(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "Cardiology", normalizedDescription);

        _specialtyReadOnlyMock
            .Setup(x => x.RecoverByStandardDescriptionAsync(normalizedDescription))
            .ReturnsAsync(existingSpecialty);

        // Act
        var result = await _useCase.RegisterSpecialtyAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Especialidade já cadastrada", result.Errors);

        _specialtyWriteOnlyMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Specialty>()), Times.Never);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task RegisterSpecialtyAsync_ShouldReturnFailure_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        var request = new RequestRegisterSpecialty("Cardiology");
        var normalizedDescription = "CARDIOLOGY";

        _specialtyReadOnlyMock
            .Setup(x => x.RecoverByStandardDescriptionAsync(normalizedDescription))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _useCase.RegisterSpecialtyAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Algo deu errado: Unexpected error", result.Errors);

        _specialtyWriteOnlyMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Specialty>()), Times.Never);
        _workUnitMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}
