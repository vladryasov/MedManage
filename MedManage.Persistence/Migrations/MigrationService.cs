using MedManage.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Migrations;

public class MigrationService
{
    private readonly MedManageDbContext _dbContext;

    public MigrationService(MedManageDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void MigrateAll()
    {
        _dbContext.Database.Migrate();
    }
}
