using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using User.Domain.Extensions;
using User.Domain.Repositories;
using User.Domain.Repositories.Contracts;
using User.Infrastructure.Repositories;
using User.Infrastructure.Repositories.User;

namespace User.Infrastructure;

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
                  .ScanIn(Assembly.Load("User.Infrastructure")).For.All());
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
            .AddScoped<IUserWriteOnly, UserRepository>()
            .AddScoped<IUserReadOnly, UserRepository>();
    }
}
