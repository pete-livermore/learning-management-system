using System.Text;
using Application.Common.Interfaces.Security;
using Application.Common.Interfaces.Token;
using Application.UseCases.Security.Interfaces;
using Infrastructure.Identity.Services;
using LearningManagementSystem.Infrastructure.Identity.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Identity;

public static class ServiceRegistration
{
    public static void AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        var securityConfig = config.GetSection(SecurityOptions.Security).Get<SecurityOptions>();
        if (securityConfig is null)
        {
            throw new InvalidOperationException(
                $"{SecurityOptions.Security} config is missing or incomplete in appsettings.json."
            );
        }

        var jwtConfig = securityConfig.Jwt;
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtConfig.SecretKey)
                    ),
                }
            );

        services.AddHttpContextAccessor();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
    }
}
