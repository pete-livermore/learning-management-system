using Application.Security.Dtos;

namespace Application.Common.Interfaces.Token;

public interface ITokenService
{
    public string Generate(TokenDataDto tokenData);
}
