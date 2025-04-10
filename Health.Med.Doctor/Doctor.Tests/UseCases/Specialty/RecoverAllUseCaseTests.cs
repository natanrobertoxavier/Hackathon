using Doctor.Application.UseCase.Specialty.Recover.RecoverAll;
using Doctor.Domain.Repositories.Contracts.Specialty;
using Moq;
using Serilog;

namespace Doctor.Tests.UseCases.Specialty;

public class RecoverAllUseCaseTests
{
    private readonly Mock<ISpecialtyReadOnly> _specialtyReadOnlyMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly RecoverAllUseCase _useCase;

    public RecoverAllUseCaseTests()
    {
        _specialtyReadOnlyMock = new Mock<ISpecialtyReadOnly>();
        _loggerMock = new Mock<ILogger>();

        _useCase = new RecoverAllUseCase(
            _specialtyReadOnlyMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task RecoverAllAsync_ShouldReturnSuccess_WhenSpecialtiesAreFound()
    {
        // Arrange
        var specialties = new List<Domain.Entities.Specialty>
        {
            new Domain.Entities.Specialty(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "Cardiology", "StandardCardiology"),
            new Domain.Entities.Specialty(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "Neurology", "StandardNeurology")
        };

        _specialtyReadOnlyMock
            .Setup(x => x.RecoverAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(specialties);

        // Act
        var result = await _useCase.RecoverAllAsync(1, 5);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(specialties.Count, result.Data.Count());
        _loggerMock.Verify(x => x.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RecoverAllAsync_ShouldReturnSuccessWithNull_WhenNoSpecialtiesAreFound()
    {
        // Arrange
        _specialtyReadOnlyMock
            .Setup(x => x.RecoverAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Domain.Entities.Specialty>());

        // Act
        var result = await _useCase.RecoverAllAsync(1, 5);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.Data);
        _loggerMock.Verify(x => x.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RecoverAllAsync_ShouldReturnFailure_WhenExceptionOccurs()
    {
        // Arrange
        _specialtyReadOnlyMock
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
