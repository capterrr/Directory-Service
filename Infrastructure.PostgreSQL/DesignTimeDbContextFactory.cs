using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.PostgreSQL;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DirectoryDbContext>
{
    public DirectoryDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DirectoryDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=directory_service;Username=postgres;Password=postgres");

        return new DirectoryDbContext(optionsBuilder.Options);
    }
}
