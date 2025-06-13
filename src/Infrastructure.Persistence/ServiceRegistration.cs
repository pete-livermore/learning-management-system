using Application.Common.Interfaces.Repositories;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories;
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
        {
            var sqlServerConnectionString =
                config.GetConnectionString("SqlServer")
                ?? throw new InvalidOperationException(
                    "ConnectionStrings:SqlServer is missing or incomplete in environment"
                );
            options.UseSqlServer(sqlServerConnectionString);
        });

        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IFilesRepository, FilesRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
