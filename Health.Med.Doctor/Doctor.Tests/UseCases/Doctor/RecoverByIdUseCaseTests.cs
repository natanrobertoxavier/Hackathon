using Doctor.Application.UseCase.Doctor.Recover.RecoverByCR;
using Doctor.Application.UseCase.Doctor.Recover.RecoverById;
using Doctor.Domain.Entities;
using Doctor.Domain.Repositories.Contracts.Doctor;
using Moq;
using Serilog;

namespace Doctor.Tests.UseCases.Doctor;
public class RecoverByIdUseCaseTests
{
    private readonly Mock<IDoctorReadOnly> _doctorReadOnlyMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly RecoverByIdUseCase _useCase;

    public RecoverByIdUseCaseTests()
    {
        _doctorReadOnlyMock = new Mock<IDoctorReadOnly>();
        _loggerMock = new Mock<ILogger>();

        _useCase = new RecoverByIdUseCase(
            _doctorReadOnlyMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task RecoverByIdAsync_ShouldReturnSuccess_WhenDoctorIsFound()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var serviceDay = new Domain.Entities.ServiceDay(doctorId, "segunda-feira", TimeSpan.FromHours(9), TimeSpan.FromHours(17));
        var specialty = new Domain.Entities.Specialty(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "Specialty 1", "SPECIALITY-1");
        var doctor = new Domain.Entities.Doctor(doctorId, DateTime.UtcNow, "Doctor 1", "Name", "email1@example.com", "123456", "password1", specialty.Id, 0.0m);

        doctor.Specialty = specialty;
        doctor.ServiceDays = new List<Domain.Entities.ServiceDay> { serviceDay };

        _doctorReadOnlyMock
            .Setup(x => x.RecoverByIdAsync(doctorId))
            .ReturnsAsync(doctor);

        // Act
        var result = await _useCase.RecoverByIdAsync(doctorId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(doctor.Name, result.Data.Name);
        Assert.Equal(doctor.CR, result.Data.CR);
        Assert.Equal(doctor.Email, result.Data.Email);
        _loggerMock.Verify(x => x.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RecoverByCRAsync_ShouldReturnSuccessWithNull_WhenDoctorIsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        _doctorReadOnlyMock
            .Setup(x => x.RecoverByIdAsync(id))
            .ReturnsAsync(new Domain.Entities.Doctor());

        // Act
        var result = await _useCase.RecoverByIdAsync(id);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.Data);
        _loggerMock.Verify(x => x.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RecoverByCRAsync_ShouldReturnFailure_WhenExceptionOccurs()
    {
        // Arrange
        var id = Guid.NewGuid();

        _doctorReadOnlyMock
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

