using Doctor.Domain.Extensions;
using Doctor.Domain.Repositories;
using Doctor.Domain.Repositories.Contracts;
using Doctor.Infrastructure.Repositories;
using Doctor.Infrastructure.Repositories.Doctor;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Doctor.Infrastructure;

public static class Initializer
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configurationManager)
    {
        AddFluentMigrator(services, configurationManager);
        AddContext(services, configurationManager);
        AddWorkUnit(services);
        AddRepositories(services);
    }

    private static void AddFluentMigrator(IServiceCollection services, IConfiguration configurationManager)
    {
        _ = bool.TryParse(configurationManager.GetSection("Settings:DatabaseInMemory").Value, out bool databaseInMemory);

        if (!databaseInMemory)
        {
            services.AddFluentMigratorCore().ConfigureRunner(c =>
                 c.AddMySql5()
                  .WithGlobalConnectionString(configurationManager.GetFullConnection())
                  .ScanIn(Assembly.Load("Doctor.Infrastructure")).For.All());
        }
    }

    private static void AddContext(IServiceCollection services, IConfiguration configurationManager)
    {
        _ = bool.TryParse(configurationManager.GetSection("Settings:DatabaseInMemory").Value, out bool databaseInMemory);

        if (!databaseInMemory)
        {
            var versaoServidor = new MySqlServerVersion(new Version(8, 0, 26));
            var connectionString = configurationManager.GetFullConnection();

            services.AddDbContext<HealthMedContext>(dbContextoOpcoes =>
            {
                dbContextoOpcoes.UseMySql(connectionString, versaoServidor);
            });
        }
    }

    private static void AddWorkUnit(IServiceCollection services)
    {
        services.AddScoped<IWorkUnit, WorkUnit>();
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services
            .AddScoped<IDoctorWriteOnly, DoctorRepository>()
            .AddScoped<IDoctorReadOnly, DoctorRepository>();
    }
}
