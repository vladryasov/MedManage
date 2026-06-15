using MedManage.Persistence.Data;
using MedManage.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Migrations;

public class MigrationService
{
    private readonly AppDbContext _dbContext;
    private readonly IEnumerable<IDataSeeder> _seeders;
    private readonly ILogger<MigrationService> _logger;

    public MigrationService(AppDbContext dbContext,
                            IEnumerable<IDataSeeder> seeders,
                            ILogger<MigrationService> logger)
    {
        _dbContext = dbContext;
        _seeders = seeders;
        _logger = logger;
    }

    public void InitializeDatabase()
    {
        try
        {
            _dbContext.Database.Migrate();

            foreach (var seeder in _seeders)
            {
                seeder.SeedAsync().GetAwaiter().GetResult();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при инициализации базы данных (миграции или сидирование)");
            throw; // Пробрасываем исключение дальше, чтобы приложение не стартовало с неготовой БД
        }
    }
}