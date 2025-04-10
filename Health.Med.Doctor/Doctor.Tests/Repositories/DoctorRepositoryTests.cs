using Doctor.Infrastructure.Repositories;
using Doctor.Infrastructure.Repositories.Doctor;
using Microsoft.EntityFrameworkCore;

namespace Doctor.Tests.Repositories;

public class DoctorRepositoryTests
{
    private readonly DbContextOptions<HealthMedContext> _dbContextOptions;

    public DoctorRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<HealthMedContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task AddAsync_ShouldAddDoctorToDatabase()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new DoctorRepository(context);
        var doctor = new Domain.Entities.Doctor(Guid.NewGuid(), DateTime.UtcNow, "Doctor Name", "email@example.com", "12345", "password", Guid.NewGuid());

        // Act
        await repository.AddAsync(doctor);
        await context.SaveChangesAsync();

        // Assert
        var addedDoctor = await context.Doctors.FirstOrDefaultAsync(d => d.Id == doctor.Id);
        Assert.NotNull(addedDoctor);
        Assert.Equal(doctor.Name, addedDoctor.Name);
    }

    [Fact]
    public async Task Update_ShouldUpdateDoctorInDatabase()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new DoctorRepository(context);
        var doctor = new Domain.Entities.Doctor(Guid.NewGuid(), DateTime.UtcNow, "Doctor Name", "email@example.com", "12345", "password", Guid.NewGuid());

        await context.Doctors.AddAsync(doctor);
        await context.SaveChangesAsync();

        // Act
        doctor.Name = "Updated Name";
        repository.Update(doctor);
        await context.SaveChangesAsync();

        // Assert
        var updatedDoctor = await context.Doctors.FirstOrDefaultAsync(d => d.Id == doctor.Id);
        Assert.NotNull(updatedDoctor);
        Assert.Equal("Updated Name", updatedDoctor.Name);
    }

    [Fact]
    public async Task RecoverByEmailAsync_ShouldReturnDoctor_WhenEmailExists()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new DoctorRepository(context);
        var doctor = new Domain.Entities.Doctor(Guid.NewGuid(), DateTime.UtcNow, "Doctor Name", "email@example.com", "12345", "password", Guid.NewGuid());

        await context.Doctors.AddAsync(doctor);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.RecoverByEmailAsync("email@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(doctor.Email, result.Email);
    }

    [Fact]
    public async Task RecoverByEmailAsync_ShouldReturnEmptyDoctor_WhenEmailDoesNotExist()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new DoctorRepository(context);

        // Act
        var result = await repository.RecoverByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task RecoverAllAsync_ShouldReturnPagedDoctors()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new DoctorRepository(context);
        var doctors = new List<Domain.Entities.Doctor>
        {
            new Domain.Entities.Doctor(Guid.NewGuid(), DateTime.UtcNow, "Doctor 1", "email1@example.com", "CR1", "password1", Guid.NewGuid()),
            new Domain.Entities.Doctor(Guid.NewGuid(), DateTime.UtcNow, "Doctor 2", "email2@example.com", "CR2", "password2", Guid.NewGuid()),
            new Domain.Entities.Doctor(Guid.NewGuid(), DateTime.UtcNow, "Doctor 3", "email3@example.com", "CR3", "password3", Guid.NewGuid())
        };

        await context.Doctors.AddRangeAsync(doctors);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.RecoverAllAsync(0, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task RecoverByIdAsync_ShouldReturnDoctor_WhenIdExists()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new DoctorRepository(context);
        var doctor = new Domain.Entities.Doctor(Guid.NewGuid(), DateTime.UtcNow, "Doctor Name", "email@example.com", "12345", "password", Guid.NewGuid());

        await context.Doctors.AddAsync(doctor);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.RecoverByIdAsync(doctor.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(doctor.Id, result.Id);
    }

    [Fact]
    public async Task RecoverByIdAsync_ShouldReturnEmptyDoctor_WhenIdDoesNotExist()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new DoctorRepository(context);

        // Act
        var result = await repository.RecoverByIdAsync(Guid.NewGuid());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Guid.Empty, result.Id);
    }
}

