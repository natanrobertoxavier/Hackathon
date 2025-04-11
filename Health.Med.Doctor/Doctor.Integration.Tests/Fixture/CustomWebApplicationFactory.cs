using Doctor.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Doctor.Integration.Tests.Fixture;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<HealthMedContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<HealthMedContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<HealthMedContext>();

            db.Database.EnsureCreated();

            SeedDatabase(db);
        });
    }

    private void SeedDatabase(HealthMedContext context)
    {
        context.Doctors.Add(new Domain.Entities.Doctor
        {
            Id = Guid.NewGuid(),
            Name = "Test Doctor",
            Email = "test@doctor.com",
            CR = "12345",
            Password = "encryptedPassword",
            SpecialtyId = Guid.NewGuid(),
            RegistrationDate = DateTime.UtcNow
        });

        context.SaveChanges();
    }
}
