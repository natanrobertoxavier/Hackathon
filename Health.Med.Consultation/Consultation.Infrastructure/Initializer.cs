using Consultation.Domain.Extensions;
using Consultation.Domain.Repositories;
using Consultation.Domain.Repositories.Contracts;
using Consultation.Domain.Services;
using Consultation.Infrastructure.Repositories;
using Consultation.Infrastructure.Repositories.Consultation;
using Consultation.Infrastructure.Services;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Consultation.Infrastructure;

public static class Initializer
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configurationManager)
    {
        AddFluentMigrator(services, configurationManager);
        AddContext(services, configurationManager);
        AddWorkUnit(services);
        AddRepositories(services);
        AddServices(services, configurationManager);
    }

    private static void AddFluentMigrator(IServiceCollection services, IConfiguration configurationManager)
    {
        _ = bool.TryParse(configurationManager.GetSection("Settings:DatabaseInMemory").Value, out bool databaseInMemory);

        if (!databaseInMemory)
        {
            services.AddFluentMigratorCore().ConfigureRunner(c =>
                 c.AddMySql5()
                  .WithGlobalConnectionString(configurationManager.GetFullConnection())
                  .ScanIn(Assembly.Load("Consultation.Infrastructure")).For.All());
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
            .AddScoped<IConsultationReadOnly, ConsultationReadOnly>()
            .AddScoped<IConsultationWriteOnly, ConsultationReadOnly>();
    }

    private static void AddServices(IServiceCollection services, IConfiguration configurationManager)
    {
        services
            .AddScoped<IUserServiceApi, UserServiceApi>()
            .AddScoped<IClientServiceApi, ClientServiceApi>();

        services.AddHttpClient("UserApi", client =>
        {
            client.BaseAddress = new Uri(configurationManager.GetSection("ServicesApiAddress:UserApi").Value);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.AddHttpClient("ClientApi", client =>
        {
            client.BaseAddress = new Uri(configurationManager.GetSection("ServicesApiAddress:ClientApi").Value);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
    }
}
