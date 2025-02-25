using Microsoft.EntityFrameworkCore;

namespace Doctor.Infrastructure.Repositories;

public class HealthMedContext(DbContextOptions<HealthMedContext> options) : DbContext(options)
{
    public DbSet<Domain.Entities.Doctor> Doctors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HealthMedContext).Assembly);
    }
}