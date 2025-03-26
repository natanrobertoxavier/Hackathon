using Microsoft.EntityFrameworkCore;

namespace Doctor.Infrastructure.Repositories;

public class HealthMedContext(DbContextOptions<HealthMedContext> options) : DbContext(options)
{
    public DbSet<Domain.Entities.Doctor> Doctors { get; set; }
    public DbSet<Domain.Entities.Specialty> Specialties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HealthMedContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }
}