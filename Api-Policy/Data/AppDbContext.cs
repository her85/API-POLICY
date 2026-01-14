using Microsoft.EntityFrameworkCore;


namespace Api_Policy.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<Claim> Claims => Set<Claim>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de precisión para montos de dinero
        modelBuilder.Entity<Policy>()
            .Property(p => p.CoverageAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Policy>()
            .Property(p => p.MonthlyPremium)
            .HasPrecision(18, 2);

        // Relación Uno a Muchos (Una póliza, muchos siniestros)
        modelBuilder.Entity<Policy>()
            .HasMany(p => p.Claims)
            .WithOne()
            .HasForeignKey(c => c.PolicyId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Seed de datos para que el portfolio no esté vacío al iniciar
        modelBuilder.Entity<Policy>().HasData(
            new Policy { 
                Id = 1, 
                PolicyNumber = "POL-F8B6B647", 
                ClientName = "Juan Ramírez", 
                StartDate = new (2024, 1, 1), 
                EndDate = new (2025, 1, 1),
                CoverageAmount = 50000,
                MonthlyPremium = 260,
                Status = PolicyStatus.Active
            }
        );
    }
}