using Doctor.Application.Services.Doctor;
using Doctor.Application.Services.User;
using Doctor.Application.UseCase.Doctor.ChangePassword;
using Doctor.Application.UseCase.Doctor.Login;
using Doctor.Application.UseCase.Doctor.Recover.RecoverAll;
using Doctor.Application.UseCase.Doctor.Recover.RecoverByCR;
using Doctor.Application.UseCase.Doctor.Register;
using Doctor.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TokenService.Manager.Controller;

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
            .AddScoped<ILoggedDoctor, LoggedDoctor>()
            .AddScoped<ILoggedUser, LoggedUser>();
    }

    private static void AddUseCases(IServiceCollection services)
    {
        services
            .AddScoped<IRegisterUseCase, RegisterUseCase>()
            .AddScoped<IRecoverAllUseCase, RecoverAllUseCase>()
            .AddScoped<IRecoverByCRUseCase, RecoverByCRUseCase>()
            .AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>()
            .AddScoped<ILoginUseCase, LoginUseCase>()
            .AddScoped<UseCase.Specialty.Register.IRegisterUseCase, UseCase.Specialty.Register.RegisterUseCase>();
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
