using MedManage.Persistence.Migrations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MedManage.Persistence.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ApplyDatabaseMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var migrationService = scope.ServiceProvider.GetRequiredService<MigrationService>();
        migrationService.MigrateAll();

        return app;
    }
}
