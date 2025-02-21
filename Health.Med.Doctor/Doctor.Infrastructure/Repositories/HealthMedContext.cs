using Microsoft.EntityFrameworkCore;

namespace Doctor.Infrastructure.Repositories;

public class HealthMedContext(DbContextOptions<HealthMedContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HealthMedContext).Assembly);
    }
}