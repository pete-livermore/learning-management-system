using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class ServiceRegistration
{
    public static void AddPersistenceInfrastructure(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.AddDbContext<LearningManagementSystemDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("SqlServer"))
        );
    }
}
