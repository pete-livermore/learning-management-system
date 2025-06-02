using Application.Dtos.Auth;

namespace Application.Interfaces.Auth;

public interface ITokenService
{
    public string Generate(TokenDataDto tokenData);
}
