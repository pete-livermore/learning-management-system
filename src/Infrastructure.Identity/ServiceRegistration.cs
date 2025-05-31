using System.Text;
using Application.Interfaces.Auth;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity;

public static class ServiceRegistration
{
    public static void AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    }
}
