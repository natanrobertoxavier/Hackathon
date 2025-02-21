using Doctor.Domain.Extensions;
using Doctor.Application;
using Doctor.Infrastructure.Migrations;
using Doctor.Infrastructure;
using Doctor.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(option => option.LowercaseUrls = true);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

UpdateDatabase();

app.MapControllers();

app.Run();

void UpdateDatabase()
{
    using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    using var context = serviceScope.ServiceProvider.GetService<HealthMedContext>();

    bool? databaseInMemory = context?.Database?.ProviderName?.Equals("Microsoft.EntityFrameworkCore.InMemory");

    if (!databaseInMemory.HasValue || !databaseInMemory.Value)
    {
        var connection = builder.Configuration.GetConnection();
        var nomeDatabase = builder.Configuration.GetDatabaseName();

        Database.CreateDatabase(connection, nomeDatabase);

        app.MigrateDatabase();
    }
}

public partial class Program { }