using Microsoft.EntityFrameworkCore;

namespace FitForge.Infrastructure.Data;

public class FitForgeDbContext : DbContext
{
    public FitForgeDbContext(DbContextOptions<FitForgeDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FitForgeDbContext).Assembly);
    }
}
