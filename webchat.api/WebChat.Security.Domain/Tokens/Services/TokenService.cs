using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebChat.Security.Domain.Configurations;
using WebChat.Security.Domain.DTOs;
using WebChat.Security.Domain.Tokens.Interfaces;

namespace WebChat.Security.Domain.Tokens.Services;

public class TokenService : ITokenService
{
    private readonly TimeSpan ExpiryDuration = new(0, 30, 0);
    private readonly IOptions<JwtSettings> _jwtOptions;

    public TokenService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtOptions = jwtOptions;
    }

    public string BuildToken(UserDTO user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Surname, user.Surname),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        if (roles.Any())
        {
            claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));
        }

        var credentials = BuildCredentials();
        var tokenDescriptor = CreateSecurityToken(claims, credentials);
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    private JwtSecurityToken CreateSecurityToken(List<Claim> claims, SigningCredentials credentials)
    {
        return new JwtSecurityToken(
            _jwtOptions.Value.Issuer,
            _jwtOptions.Value.Audience,
            claims,
            expires: DateTime.Now.Add(ExpiryDuration),
            signingCredentials: credentials
        );
    }

    private SigningCredentials BuildCredentials()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        return credentials;
    }
}
