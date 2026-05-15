using Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Extensions;

public static class WebApplicationExtensions
{
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<DirectoryDbContext>();

        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<DirectoryDbContext>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }
}
