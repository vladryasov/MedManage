using MedManage.Application.Extensions;
using MedManage.Identity.Extensions;
using MedManage.Indentity.Extensions;
using MedManage.Persistence.Extensions;
using MedManage.WebAPI.BackgroundServices;
using UserManagement.Application.Filters.ExceptionHadling;

public class Program
{
    public static void Main(string[] args)
    {
        DotNetEnv.Env.Load();

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddPersistenceServices(builder.Configuration);
        builder.Services.AddCoreApplicationServices();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddInfrastructureIdentityServices(builder.Configuration);
        builder.Services.AddDirectoryBrowser();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSwaggerAuthentication();

        builder.Services.AddHostedService<OutboxProcessorService>();
        builder.Services.AddHostedService<OutboxCleanupService>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                policy => policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        var app = builder.Build();

        app.InitializeDatabase();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "MedManage WebAPI");
            options.RoutePrefix = string.Empty;
        });
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
