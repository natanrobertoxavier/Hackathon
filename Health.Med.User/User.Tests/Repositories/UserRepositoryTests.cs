using Microsoft.EntityFrameworkCore;
using User.Infrastructure.Repositories;
using User.Infrastructure.Repositories.User;

namespace User.Tests.Repositories;

public class UserRepositoryTests
{
    private readonly DbContextOptions<HealthMedContext> _dbContextOptions;

    public UserRepositoryTests()
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
        var repository = new UserRepository(context);
        var user = new Domain.Entities.User("John Doe", "john.doe@example.com", "password123");

        // Act
        await repository.AddAsync(user);
        await context.SaveChangesAsync();

        // Assert
        var addedDoctor = await context.Users.FirstOrDefaultAsync(d => d.Id == user.Id);
        Assert.NotNull(addedDoctor);
        Assert.Equal(user.Name, addedDoctor.Name);
    }

    [Fact]
    public async Task Update_ShouldUpdateDoctorInDatabase()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new UserRepository(context);
        var user = new Domain.Entities.User("John Doe", "john.doe@example.com", "password123");

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        user.Name = "Updated Name";
        repository.Update(user);
        await context.SaveChangesAsync();

        // Assert
        var updatedDoctor = await context.Users.FirstOrDefaultAsync(d => d.Id == user.Id);
        Assert.NotNull(updatedDoctor);
        Assert.Equal("Updated Name", updatedDoctor.Name);
    }

    [Fact]
    public async Task RecoverByEmailAsync_ShouldReturnDoctor_WhenEmailExists()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new UserRepository(context);
        var user = new Domain.Entities.User("John Doe", "email@example.com", "password123");

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.RecoverByEmailAsync("email@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task RecoverByEmailAsync_ShouldReturnEmptyDoctor_WhenEmailDoesNotExist()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new UserRepository(context);

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
        var repository = new UserRepository(context);
        var doctors = new List<Domain.Entities.User>
        {
            new Domain.Entities.User("User 1", "john.doe@example.com", "password123"),
            new Domain.Entities.User("User 2", "john.doe@example.com", "password123"),
            new Domain.Entities.User("User 3", "john.doe@example.com", "password123"),
        };

        await context.Users.AddRangeAsync(doctors);
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
        var repository = new UserRepository(context);
        var user = new Domain.Entities.User("John Doe", "john.doe@example.com", "password123");

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.RecoverByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task RecoverByIdAsync_ShouldReturnEmptyDoctor_WhenIdDoesNotExist()
    {
        // Arrange
        var context = new HealthMedContext(_dbContextOptions);
        var repository = new UserRepository(context);

        // Act
        var result = await repository.RecoverByIdAsync(Guid.NewGuid());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Guid.Empty, result.Id);
    }
}

