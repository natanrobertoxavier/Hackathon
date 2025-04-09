using Microsoft.EntityFrameworkCore;

namespace Client.Infrastructure.Repositories;

public class HealthMedContext(DbContextOptions<HealthMedContext> options) : DbContext(options)
{
    public DbSet<Domain.Entities.Client> Clients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HealthMedContext).Assembly);
    }
}