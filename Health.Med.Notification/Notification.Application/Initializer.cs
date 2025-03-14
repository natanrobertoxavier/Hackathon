using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.UseCase.SendMail;
using Notification.Infrastructure.Settings;
using Serilog;

namespace Notification.Application;

public static class Initializer
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddUseCases(services);
        AddSerilog(services);
        ConfiguraMailSettings(services, configuration);
    }

    private static void AddUseCases(IServiceCollection services)
    {
        services
            .AddScoped<ISendMailUseCase, SendMailUseCase>();
    }

    private static void AddSerilog(IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        services.AddSingleton(Log.Logger);
    }

    private static IServiceCollection ConfiguraMailSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MailSettings>(options => configuration.GetSection(nameof(MailSettings)).Bind(options));
        return services;
    }
}
