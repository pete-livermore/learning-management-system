namespace WebApi;

using Application;
using Infrastructure.Identity;
using Infrastructure.Identity.Models;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Shared;
using Infrastructure.Uploads;
using Microsoft.AspNetCore.Identity;
using WebApi.Extensions;
using WebApi.Services;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddConfigOptions(Configuration);
        services.AddApplicationLayer();
        services.AddPersistenceInfrastructure(Configuration);
        services
            .AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<LearningManagementSystemDbContext>()
            .AddDefaultTokenProviders();
        services.AddIdentityInfrastructure(Configuration);
        services.AddSharedInfrastructure(Configuration);
        services.AddUploadsInfrastructure();
        services.AddControllers();
        services.AddApiVersioningExtension();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseExceptionHandler();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
