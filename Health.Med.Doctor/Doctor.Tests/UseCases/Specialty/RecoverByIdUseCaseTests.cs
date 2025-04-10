using Doctor.Application.UseCase.Specialty.Recover.RecoverById;
using Doctor.Domain.Repositories.Contracts.Specialty;
using Moq;
using Serilog;

namespace Doctor.Tests.UseCases.Specialty;
public class RecoverByIdUseCaseTests
{
    private readonly Mock<ISpecialtyReadOnly> _specialtyReadOnlyMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly RecoverByIdUseCase _useCase;

    public RecoverByIdUseCaseTests()
    {
        _specialtyReadOnlyMock = new Mock<ISpecialtyReadOnly>();
        _loggerMock = new Mock<ILogger>();

        _useCase = new RecoverByIdUseCase(
            _specialtyReadOnlyMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task RecoverByIdAsync_ShouldReturnSuccess_WhenSpecialtyIsFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var specialty = new Domain.Entities.Specialty(id, DateTime.UtcNow, Guid.NewGuid(), "Cardiology", "StandardCardiology");

        _specialtyReadOnlyMock
            .Setup(x => x.RecoverByIdAsync(id))
            .ReturnsAsync(specialty);

        // Act
        var result = await _useCase.RecoverByIdAsync(id);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(specialty.Description, result.Data.Description);
        Assert.Equal(specialty.StandardDescription, result.Data.StandardDescription);
        _loggerMock.Verify(x => x.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RecoverByIdAsync_ShouldReturnSuccessWithNull_WhenSpecialtyIsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        _specialtyReadOnlyMock
            .Setup(x => x.RecoverByIdAsync(id))
            .ReturnsAsync(new Domain.Entities.Specialty());

        // Act
        var result = await _useCase.RecoverByIdAsync(id);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.Data);
        _loggerMock.Verify(x => x.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RecoverByIdAsync_ShouldReturnFailure_WhenExceptionOccurs()
    {
        // Arrange
        var id = Guid.NewGuid();

        _specialtyReadOnlyMock
            .Setup(x => x.RecoverByIdAsync(id))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _useCase.RecoverByIdAsync(id);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Algo deu errado: Unexpected error", result.Errors);
        _loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }
}
