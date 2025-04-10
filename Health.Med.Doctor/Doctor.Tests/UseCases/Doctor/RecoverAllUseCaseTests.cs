using Doctor.Application.UseCase.Doctor.Recover.RecoverAll;
using Doctor.Domain.Entities;
using Doctor.Domain.Repositories.Contracts.Doctor;
using Doctor.Domain.Repositories.Contracts.Specialty;
using Moq;
using Serilog;

namespace Doctor.Tests.UseCases.Doctor;

public class RecoverAllUseCaseTests
{
    private readonly Mock<IDoctorReadOnly> _doctorReadOnlyMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly RecoverAllUseCase _useCase;

    public RecoverAllUseCaseTests()
    {
        _doctorReadOnlyMock = new Mock<IDoctorReadOnly>();
        _loggerMock = new Mock<ILogger>();

        _useCase = new RecoverAllUseCase(
            _doctorReadOnlyMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task RecoverAllAsync_ShouldReturnSuccess_WhenDoctorsAreFound()
    {
        // Arrange
        var specialty = new Domain.Entities.Specialty(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "Specialty 1", "SPECIALITY-1");
        var doctors = new List<Domain.Entities.Doctor>
        {
            new Domain.Entities.Doctor(Guid.NewGuid(), DateTime.UtcNow, "Doctor 1", "email1@example.com", "CR1", "password1", specialty.Id),
            new Domain.Entities.Doctor(Guid.NewGuid(), DateTime.UtcNow, "Doctor 2", "email2@example.com", "CR2", "password2", specialty.Id)
        };

        doctors[0].Specialty = specialty;
        doctors[1].Specialty = specialty;

        _doctorReadOnlyMock
            .Setup(x => x.RecoverAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(doctors);

        // Act
        var result = await _useCase.RecoverAllAsync(1, 5);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(doctors.Count, result.Data.Count());
        _loggerMock.Verify(x => x.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RecoverAllAsync_ShouldReturnSuccessWithNull_WhenNoDoctorsAreFound()
    {
        // Arrange
        _doctorReadOnlyMock
            .Setup(x => x.RecoverAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Domain.Entities.Doctor>());

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
        _doctorReadOnlyMock
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
