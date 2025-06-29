﻿using Doctor.Application.UseCase.Doctor.Recover.RecoverByCR;
using Doctor.Domain.Entities;
using Doctor.Domain.Repositories.Contracts.Doctor;
using Moq;
using Serilog;

namespace Doctor.Tests.UseCases.Doctor;
public class RecoverByCRUseCaseTests
{
    private readonly Mock<IDoctorReadOnly> _doctorReadOnlyMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly RecoverByCRUseCase _useCase;

    public RecoverByCRUseCaseTests()
    {
        _doctorReadOnlyMock = new Mock<IDoctorReadOnly>();
        _loggerMock = new Mock<ILogger>();

        _useCase = new RecoverByCRUseCase(
            _doctorReadOnlyMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task RecoverByCRAsync_ShouldReturnSuccess_WhenDoctorIsFound()
    {
        // Arrange
        var cr = "12345";
        var doctorId = Guid.NewGuid();
        var serviceDay = new Domain.Entities.ServiceDay(doctorId, "segunda-feira", TimeSpan.FromHours(9), TimeSpan.FromHours(17));
        var specialty = new Domain.Entities.Specialty(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "Specialty 1", "SPECIALITY-1");
        var doctor = new Domain.Entities.Doctor(doctorId, DateTime.UtcNow, "Doctor 1", "Name1", "email1@example.com", "CR1", "password1", specialty.Id, 0.0m);

        doctor.Specialty = specialty;
        doctor.ServiceDays = new List<Domain.Entities.ServiceDay> { serviceDay };

        _doctorReadOnlyMock
            .Setup(x => x.RecoverByCRAsync(cr))
            .ReturnsAsync(doctor);

        // Act
        var result = await _useCase.RecoverByCRAsync(cr);

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
        var cr = "12345";

        _doctorReadOnlyMock
            .Setup(x => x.RecoverByCRAsync(cr))
            .ReturnsAsync(new Domain.Entities.Doctor());

        // Act
        var result = await _useCase.RecoverByCRAsync(cr);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.Data);
        _loggerMock.Verify(x => x.Information(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RecoverByCRAsync_ShouldReturnFailure_WhenExceptionOccurs()
    {
        // Arrange
        var cr = "12345";

        _doctorReadOnlyMock
            .Setup(x => x.RecoverByCRAsync(cr))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _useCase.RecoverByCRAsync(cr);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Algo deu errado: Unexpected error", result.Errors);
        _loggerMock.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }
}

