using Microsoft.AspNetCore.Builder;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Doctor.Persistence.Infrastructure.Migrations;

public static class MigrationExtension
{
    public static void MigrateDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.ListMigrations();

        runner.MigrateUp();
    }
}
