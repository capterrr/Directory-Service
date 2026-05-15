using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.PostgreSQL;

public class DirectoryDbContext : DbContext
{
    public DirectoryDbContext(DbContextOptions<DirectoryDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DirectoryDbContext).Assembly);
    }

    public DbSet<Position> Positions { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<DepartmentPosition> DepartmentPositions { get; set; }
    public DbSet<DepartmentLocation> DepartmentLocations { get; set; }
}