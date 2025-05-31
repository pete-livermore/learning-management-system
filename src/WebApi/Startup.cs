namespace WebApi;

using Application;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Shared;
using WebApi.Extensions;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationLayer();
        services.AddPersistenceInfrastructure(Configuration);
        services.AddSharedInfrastructure(Configuration);
        services.AddIdentityInfrastructure(Configuration);
        services.AddControllers();
        services.AddApiVersioningExtension();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UsePathBase(new PathString("/api"));
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
