using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces.Token;
using Application.Security.Dtos;
using LearningManagementSystem.Infrastructure.Identity.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Identity.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtConfig;

        public TokenService(IOptions<SecurityOptions> securityConfig)
        {
            _jwtConfig = securityConfig.Value.Jwt;
        }

        public string Generate(TokenDataDto tokenData)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtConfig.SecretKey)
            );
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, tokenData.Email),
                new(ClaimTypes.NameIdentifier, tokenData.UserId.ToString()),
            };

            claims.AddRange(tokenData.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtConfig.ExpiryInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
