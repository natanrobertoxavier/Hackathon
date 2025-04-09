using Microsoft.EntityFrameworkCore;

namespace User.Infrastructure.Repositories;

public class HealthMedContext(DbContextOptions<HealthMedContext> options) : DbContext(options)
{
    public DbSet<Domain.Entities.User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HealthMedContext).Assembly);
    }
}