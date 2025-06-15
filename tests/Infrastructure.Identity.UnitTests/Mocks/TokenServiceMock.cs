using System;
using Infrastructure.Identity.Services;
using LearningManagementSystem.Infrastructure.Identity.Configuration;
using Microsoft.Extensions.Options;
using Moq;

namespace Infrastructure.Identity.UnitTests.Mocks;

public static class TokenServiceMock
{
    private class SecurityOptionsWrapper : IOptions<SecurityOptions>
    {
        public required SecurityOptions Value { get; set; }
    }

    public static Mock<TokenService> GetMock()
    {
        var passwordOptions = new PasswordOptions();
        var jwtOptions = new JwtOptions()
        {
            SecretKey = "",
            Audience = "",
            ExpiryInMinutes = 1000,
            Issuer = "",
        };
        var securityOptions = new SecurityOptions()
        {
            Password = passwordOptions,
            Jwt = jwtOptions,
        };
        IOptions<SecurityOptions> securityOptionsWrapper = new SecurityOptionsWrapper()
        {
            Value = securityOptions,
        };
        return new Mock<TokenService>(securityOptionsWrapper);
    }
}
