using Consultation.Domain.Entities;
using Consultation.Infrastructure.Repositories;
using Consultation.Infrastructure.Repositories.Consultation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Consultation.Tests.Repositories;

public class ConsultationRepositoryTests
{
    private readonly DbContextOptions<HealthMedContext> _dbContextOptions;

    public ConsultationRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<HealthMedContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task AddAsync_ShouldAddConsultationToDatabase()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new ConsultationRepository(context);
        var consultation = new Domain.Entities.Consultation(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);

        // Act
        await repository.AddAsync(consultation);
        await context.SaveChangesAsync();

        // Assert
        var addedConsultation = await context.Consultations.FirstOrDefaultAsync(c => c.Id == consultation.Id);
        Assert.NotNull(addedConsultation);
        Assert.Equal(consultation.ClientId, addedConsultation.ClientId);
    }

    [Fact]
    public async Task ThereIsConsultationForDoctor_ShouldReturnTrue_WhenConsultationExists()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new ConsultationRepository(context);
        var doctorId = Guid.NewGuid();
        var consultationDate = DateTime.UtcNow;

        var consultation = new Domain.Entities.Consultation(Guid.NewGuid(), doctorId, consultationDate, true);
        await context.Consultations.AddAsync(consultation);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ThereIsConsultationForDoctor(doctorId, consultationDate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ThereIsConsultationForDoctor_ShouldReturnFalse_WhenConsultationDoesNotExist()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new ConsultationRepository(context);
        var doctorId = Guid.NewGuid();
        var consultationDate = DateTime.UtcNow;

        // Act
        var result = await repository.ThereIsConsultationForDoctor(doctorId, consultationDate);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ThereIsConsultationForClient_ShouldReturnTrue_WhenConsultationExists()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new ConsultationRepository(context);
        var clientId = Guid.NewGuid();
        var consultationDate = DateTime.UtcNow;

        var consultation = new Domain.Entities.Consultation(clientId, Guid.NewGuid(), consultationDate, true);
        await context.Consultations.AddAsync(consultation);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ThereIsConsultationForClient(clientId, consultationDate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ThereIsConsultationForClient_ShouldReturnFalse_WhenConsultationDoesNotExist()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new ConsultationRepository(context);
        var clientId = Guid.NewGuid();
        var consultationDate = DateTime.UtcNow;

        // Act
        var result = await repository.ThereIsConsultationForClient(clientId, consultationDate);

        // Assert
        Assert.False(result);
    }
}
