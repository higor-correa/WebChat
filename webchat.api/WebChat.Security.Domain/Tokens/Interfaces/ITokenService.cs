using WebChat.Security.Domain.DTOs;

namespace WebChat.Security.Domain.Tokens.Interfaces;

public interface ITokenService
{
    string BuildToken(UserDTO user, IEnumerable<string> roles);
}