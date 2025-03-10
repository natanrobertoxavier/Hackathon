using Client.Application.UseCase.Login;
using Client.Application.UseCase.Register;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TokenService.Manager.Controller;

namespace Client.Application;

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
        //services
        //    .AddScoped<ILoggedUser, LoggedUser>();
    }

    private static void AddUseCases(IServiceCollection services)
    {
        services
            .AddScoped<IRegisterUseCase, RegisterUseCase>()
            //.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>()
            //.AddScoped<IRecoverAllUseCase, RecoverAllUseCase>()
            //.AddScoped<IRecoverByEmailUseCase, RecoverByEmailUseCase>()
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
