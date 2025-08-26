using Microsoft.EntityFrameworkCore;
using OrganizationService.Entities;

namespace OrganizationService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<OrganizationNode> OrganizationNodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Indexek a gyorsabb nested set lekérdezésekhez
        modelBuilder.Entity<OrganizationNode>()
            .HasIndex(n => new { n.Lft, n.Rgt });
            
        modelBuilder.Entity<OrganizationNode>()
            .HasIndex(n => n.Lft);

        modelBuilder.Entity<OrganizationNode>()
            .HasIndex(n => n.Rgt);

        // Gyökér elem létrehozása, ha a tábla üres
        modelBuilder.Entity<OrganizationNode>().HasData(
            new OrganizationNode
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-a7b8-c9d0-e1f2a3b4c5d6"),
                Name = "ROOT",
                Description = "The root of the organization",
                Lft = 1,
                Rgt = 2,
                Level = 0,
                ParentId = null
            }
        );
    }
}
