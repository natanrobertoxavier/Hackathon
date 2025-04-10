using Doctor.Domain.Entities;
using Doctor.Infrastructure.Repositories;
using Doctor.Infrastructure.Repositories.Specialty;
using Microsoft.EntityFrameworkCore;

namespace Doctor.Tests.Repositories;

public class SpecialtyRepositoryTests
{
    private readonly DbContextOptions<HealthMedContext> _dbContextOptions;

    public SpecialtyRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<HealthMedContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task AddAsync_ShouldAddSpecialtyToDatabase()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new SpecialtyRepository(context);
        var specialty = new Specialty(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "Cardiology", "StandardCardiology");

        // Act
        await repository.AddAsync(specialty);
        await context.SaveChangesAsync();

        // Assert
        var addedSpecialty = await context.Specialties.FirstOrDefaultAsync(s => s.Id == specialty.Id);
        Assert.NotNull(addedSpecialty);
        Assert.Equal(specialty.Description, addedSpecialty.Description);
    }

    [Fact]
    public async Task Update_ShouldUpdateSpecialtyInDatabase()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new SpecialtyRepository(context);
        var specialty = new Specialty(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "Cardiology", "StandardCardiology");

        await context.Specialties.AddAsync(specialty);
        await context.SaveChangesAsync();

        // Act
        specialty.Description = "Updated Cardiology";
        repository.Update(specialty);
        await context.SaveChangesAsync();

        // Assert
        var updatedSpecialty = await context.Specialties.FirstOrDefaultAsync(s => s.Id == specialty.Id);
        Assert.NotNull(updatedSpecialty);
        Assert.Equal("Updated Cardiology", updatedSpecialty.Description);
    }

    [Fact]
    public async Task RecoverByIdAsync_ShouldReturnSpecialty_WhenIdExists()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new SpecialtyRepository(context);
        var specialty = new Specialty(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "Cardiology", "StandardCardiology");

        await context.Specialties.AddAsync(specialty);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.RecoverByIdAsync(specialty.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(specialty.Id, result.Id);
    }

    [Fact]
    public async Task RecoverByIdAsync_ShouldReturnEmptySpecialty_WhenIdDoesNotExist()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new SpecialtyRepository(context);

        // Act
        var result = await repository.RecoverByIdAsync(Guid.NewGuid());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task RecoverByStandardDescriptionAsync_ShouldReturnSpecialty_WhenDescriptionExists()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new SpecialtyRepository(context);
        var specialty = new Specialty(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "Cardiology", "StandardCardiology");

        await context.Specialties.AddAsync(specialty);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.RecoverByStandardDescriptionAsync("StandardCardiology");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(specialty.StandardDescription, result.StandardDescription);
    }

    [Fact]
    public async Task RecoverByStandardDescriptionAsync_ShouldReturnEmptySpecialty_WhenDescriptionDoesNotExist()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new SpecialtyRepository(context);

        // Act
        var result = await repository.RecoverByStandardDescriptionAsync("NonExistentDescription");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task RecoverAllAsync_ShouldReturnPagedSpecialties()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new SpecialtyRepository(context);
        var specialties = new List<Specialty>
        {
            new Specialty(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "Cardiology", "StandardCardiology"),
            new Specialty(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "Neurology", "StandardNeurology"),
            new Specialty(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "Dermatology", "StandardDermatology")
        };

        await context.Specialties.AddRangeAsync(specialties);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.RecoverAllAsync(0, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}
