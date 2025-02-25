using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TokenService.Manager.Controller;
using Serilog;
using Doctor.Application.UseCase.Register;
using Doctor.Application.UseCase.Recover.RecoverAll;
using Doctor.Application.UseCase.Recover.RecoverByCR;
using Doctor.Domain.Repositories.Contracts;
using Doctor.Application.Services;
using Doctor.Application.UseCase.Login;

namespace Doctor.Application;

public static class Initializer
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddLoggedUsers(services);
        AddUseCases(services);
        AddAdditionalKeyPassword(services, configuration);
        AddJWTToken(services, configuration);
        AddSerilog(services);
    }

    private static void AddLoggedUsers(IServiceCollection services)
    {
        services
            .AddScoped<ILoggedDoctor, LoggedDoctor>();
    }

    private static void AddUseCases(IServiceCollection services)
    {
        services
            .AddScoped<IRegisterUseCase, RegisterUseCase>()
            .AddScoped<IRecoverAllUseCase, RecoverAllUseCase>()
            .AddScoped<IRecoverByCRUseCase, RecoverByCRUseCase>()
            .AddScoped<ILoginUseCase, LoginUseCase>();
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
}
