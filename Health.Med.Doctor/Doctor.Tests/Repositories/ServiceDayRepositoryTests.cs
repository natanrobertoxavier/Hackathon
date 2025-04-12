using Doctor.Domain.Entities;
using Doctor.Infrastructure.Repositories;
using Doctor.Infrastructure.Repositories.ServiceDay;
using Microsoft.EntityFrameworkCore;

namespace Doctor.Tests.Repositories;
public class ServiceDayRepositoryTests
{
    private readonly DbContextOptions<HealthMedContext> _dbContextOptions;

    public ServiceDayRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<HealthMedContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task AddAsync_ShouldAddServiceDay()
    {
        // Arrange
        using var context = new HealthMedContext(_dbContextOptions);
        var repository = new ServiceDayRepository(context);

        var serviceDay = new ServiceDay
        {
            Id = Guid.NewGuid(),
            RegistrationDate = DateTime.UtcNow,
            DoctorId = Guid.NewGuid(),
            Day = "Monday",
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(17)
        };

        // Act
        await repository.AddAsync(serviceDay);
        await context.SaveChangesAsync();

        // Assert
        var result = await context.ServiceDays.FirstOrDefaultAsync();
        Assert.NotNull(result);
        Assert.Equal(serviceDay.Day, result.Day);
    }

    [Fact]
    public async Task AddAsync_ShouldAddMultipleServiceDays()
    {
        // Arrange
        using var context = new HealthMedContext(_dbContextOptions);
        var repository = new ServiceDayRepository(context);

        var serviceDays = new List<ServiceDay>
        {
            new ServiceDay
            {
                Id = Guid.NewGuid(),
                RegistrationDate = DateTime.UtcNow,
                DoctorId = Guid.NewGuid(),
                Day = "Monday",
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(17)
            },
            new ServiceDay
            {
                Id = Guid.NewGuid(),
                RegistrationDate = DateTime.UtcNow,
                DoctorId = Guid.NewGuid(),
                Day = "Tuesday",
                StartTime = TimeSpan.FromHours(10),
                EndTime = TimeSpan.FromHours(18)
            }
        };

        // Act
        await repository.AddAsync(serviceDays);
        await context.SaveChangesAsync();

        // Assert
        var result = await context.ServiceDays.ToListAsync();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Remove_ShouldRemoveServiceDays()
    {
        // Arrange
        using var context = new HealthMedContext(_dbContextOptions);
        var repository = new ServiceDayRepository(context);

        var doctorId = Guid.NewGuid();
        var serviceDays = new List<ServiceDay>
        {
            new ServiceDay
            {
                Id = Guid.NewGuid(),
                RegistrationDate = DateTime.UtcNow,
                DoctorId = doctorId,
                Day = "Monday",
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(17)
            },
            new ServiceDay
            {
                Id = Guid.NewGuid(),
                RegistrationDate = DateTime.UtcNow,
                DoctorId = doctorId,
                Day = "Tuesday",
                StartTime = TimeSpan.FromHours(10),
                EndTime = TimeSpan.FromHours(18)
            }
        };

        context.ServiceDays.AddRange(serviceDays);
        context.SaveChanges();

        // Act
        repository.Remove(doctorId, new List<string> { "Monday" });
        context.SaveChanges();

        // Assert
        var result = context.ServiceDays.ToList();
        Assert.Single(result);
        Assert.Equal("Tuesday", result.First().Day);
    }

    [Fact]
    public async Task GetByDoctorIdAsync_ShouldReturnServiceDays()
    {
        // Arrange
        using var context = new HealthMedContext(_dbContextOptions);
        var repository = new ServiceDayRepository(context);

        var doctorId = Guid.NewGuid();
        var serviceDays = new List<ServiceDay>
        {
            new ServiceDay
            {
                Id = Guid.NewGuid(),
                RegistrationDate = DateTime.UtcNow,
                DoctorId = doctorId,
                Day = "Monday",
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(17)
            },
            new ServiceDay
            {
                Id = Guid.NewGuid(),
                RegistrationDate = DateTime.UtcNow,
                DoctorId = doctorId,
                Day = "Tuesday",
                StartTime = TimeSpan.FromHours(10),
                EndTime = TimeSpan.FromHours(18)
            }
        };

        context.ServiceDays.AddRange(serviceDays);
        context.SaveChanges();

        // Act
        var result = await repository.GetByDoctorIdAsync(doctorId);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByDoctorIdAndDaysAsync_ShouldReturnFilteredServiceDays()
    {
        // Arrange
        using var context = new HealthMedContext(_dbContextOptions);
        var repository = new ServiceDayRepository(context);

        var doctorId = Guid.NewGuid();
        var serviceDays = new List<ServiceDay>
        {
            new ServiceDay
            {
                Id = Guid.NewGuid(),
                RegistrationDate = DateTime.UtcNow,
                DoctorId = doctorId,
                Day = "Monday",
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(17)
            },
            new ServiceDay
            {
                Id = Guid.NewGuid(),
                RegistrationDate = DateTime.UtcNow,
                DoctorId = doctorId,
                Day = "Tuesday",
                StartTime = TimeSpan.FromHours(10),
                EndTime = TimeSpan.FromHours(18)
            }
        };

        context.ServiceDays.AddRange(serviceDays);
        context.SaveChanges();

        // Act
        var result = await repository.GetByDoctorIdAndDaysAsync(doctorId, new List<string> { "Monday" });

        // Assert
        Assert.Single(result);
        Assert.Equal("Monday", result.First().Day);
    }
}
