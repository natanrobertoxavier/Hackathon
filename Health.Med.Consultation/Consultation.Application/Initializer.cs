using Consultation.Application.Contracts;
using Consultation.Application.Messages;
using Consultation.Application.Messages.Handlers;
using Consultation.Application.Services;
using Consultation.Application.Services.LoggedClientService;
using Consultation.Application.Services.LoggedDoctor;
using Consultation.Application.Settings;
using Consultation.Application.UseCase.Consultation.ClientCancel;
using Consultation.Application.UseCase.Consultation.Confirm;
using Consultation.Application.UseCase.Consultation.Recover.RecoverByDoctorId;
using Consultation.Application.UseCase.Consultation.Refuse;
using Consultation.Application.UseCase.Consultation.Register;
using Consultation.Application.UseCase.Consultation.Validate;
using Consultation.Application.UseCase.SendEmailClient;
using Consultation.Application.UseCase.SendEmailDoctor;
using Consultation.Domain.Messages.DomainEvents;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TokenService.Manager.Controller;

namespace Consultation.Application;

public static class Initializer
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddLoggedUsers(services);
        AddUseCases(services);
        AddAdditionalKeyPassword(services, configuration);
        AddJWTToken(services, configuration);
        AddSerilog(services);
        ConfiguraMailSettings(services, configuration);
        AddMessagesEvents(services);
        AddEvents(services);
        AddPolicyCors(services);
    }

    private static void AddLoggedUsers(IServiceCollection services)
    {
        services
            .AddScoped<ILoggedClient, LoggedClient>()
            .AddScoped<ILoggedDoctor, LoggedDoctor>();
    }

    private static void AddUseCases(IServiceCollection services)
    {
        services
            .AddScoped<IRegisterUseCase, RegisterUseCase>()
            .AddScoped<IRecoverByDoctorIdUseCase, RecoverByDoctorIdUseCase>()
            .AddScoped<ISendEmailClientUseCase, SendEmailClientUseCase>()
            .AddScoped<ISendEmailDoctorUseCase, SendEmailDoctorUseCase>()
            .AddScoped<IAcceptUseCase, AcceptUseCase>()
            .AddScoped<IRefuseUseCase, RefuseUseCase>()
            .AddScoped<IValidateUseCase, ValidateUseCase>()
            .AddScoped<IClientCancelUseCase, ClientCancelUseCase>();
    }

    private static void AddAdditionalKeyPassword(IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection("Settings:Password:AdditionalKeyPassword");
        services.AddScoped(option => new PasswordEncryptor(section.Value));
    }

    private static void AddJWTToken(IServiceCollection services, IConfiguration configuration)
    {
        var sectionLifeTime = configuration.GetRequiredSection("Settings:Jwt:LifeTimeTokenMinutes");
        var sectionKey = configuration.GetRequiredSection("Settings:Jwt:KeyToken");
        services.AddScoped(option => new TokenController(int.Parse(sectionLifeTime.Value), sectionKey.Value));
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
        services.Configure<TemplateSettings>(options => configuration.GetSection(nameof(TemplateSettings)).Bind(options));
        return services;
    }

    private static void AddMessagesEvents(IServiceCollection services)
    {
        services.AddScoped<IMessagePublisher, MessagePublisher>();
        services.AddScoped<INotificationHandler<SendEmailClientEvent>, SendEmailConfirmationClientEventHandler>();
    }

    private static void AddEvents(IServiceCollection services)
    {
        services.AddScoped<IEventAppService, EventsAppService>();
    }

    private static void AddPolicyCors(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder
                    .WithOrigins("http://191.252.179.169")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }
}
