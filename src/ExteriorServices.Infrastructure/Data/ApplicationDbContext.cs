using ExteriorServices.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExteriorServices.Infrastructure.Data;

public class ExteriorServicesDbContext : DbContext
{
    public ExteriorServicesDbContext(
        DbContextOptions<ExteriorServicesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Property> Properties => Set<Property>();

    public DbSet<Job> Jobs => Set<Job>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExteriorServicesDbContext).Assembly);
    }
}