using Microsoft.EntityFrameworkCore;
using Ticketing.Query.Infrastructure.Persistence;

namespace Ticketing.Query.Application.Extensions;

public static class MigrationExtension
{
    // extensión para que cada que exista una migración por hacer, que se ejecute
    public static async Task ApplyMigration(this IApplicationBuilder app)
    {
        // el encargado de ejecutar la migracion es la instancia de db context
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var service = scope.ServiceProvider;
            var loggerFactory = service.GetRequiredService<ILoggerFactory>();

            try
            {
                // el contextfactory es el que me crea una instancia del dbcontext
                var contextFactory = service.GetRequiredService<DatabaseContextFactory>();
                using TicketDbContext dbContext = contextFactory.CreateDbContext();
                await dbContext.Database.MigrateAsync();
            }
            catch (Exception e)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(e, "An error occurred seeding the DB.");
            }
        }
    }
}