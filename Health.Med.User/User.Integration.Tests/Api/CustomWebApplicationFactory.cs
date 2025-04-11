using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TokenService.Manager.Controller;
using User.Infrastructure.Repositories;

namespace User.Integration.Tests.Api;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private ServiceProvider _serviceProvider;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            ConfigureInMemoryDatabase(services);
            SeedTestData();
        });

        builder.UseEnvironment("IntegrationTests");

        builder.ConfigureAppConfiguration((_, appConfiguration) =>
        {
            appConfiguration.AddJsonFile("appsettings.IntegrationTests.json", optional: false, reloadOnChange: true);
        });
    }

    private void ConfigureInMemoryDatabase(IServiceCollection services)
    {
        RemoveExistingDatabaseConfiguration(services);

        var provider = services
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        services.AddDbContext<HealthMedContext>(options =>
        {
            options.UseInMemoryDatabase("InMemoryDbForTesting");
            options.UseInternalServiceProvider(provider);
        });

        _serviceProvider = services.BuildServiceProvider();

        using var scope = _serviceProvider.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<HealthMedContext>();
        database.Database.EnsureDeleted();
        database.Database.EnsureCreated();
    }

    private static void RemoveExistingDatabaseConfiguration(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<HealthMedContext>));

        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }

    private void SeedTestData()
    {
        using var context = GetContext();

        var passwordEncryptor = CreatePasswordEncryptor();

        var user = new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            RegistrationDate = DateTime.UtcNow,
            Name = "Test User",
            Email = "testuser@example.com",
            Password = passwordEncryptor.Encrypt("123456"),
        };

        context.Users.Add(user);

        context.SaveChanges();
    }

    private HealthMedContext GetContext() =>
        _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<HealthMedContext>();

    private static PasswordEncryptor CreatePasswordEncryptor()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.IntegrationTests.json", optional: false, reloadOnChange: true)
            .Build();

        var additionalKeyPassword = configuration["Settings:Password:AdditionalKeyPassword"];
        return new PasswordEncryptor(additionalKeyPassword);
    }
}
