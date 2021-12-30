using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CtServer.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CtServer.Services;

public class TokenService
{
    private readonly SymmetricSecurityKey _key;

    public TokenService(IOptions<JwtOptions> jwtOptions)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(jwtOptions.Value.Secret);
        _key = new SymmetricSecurityKey(bytes);
    }

    public string CreateToken(int userId)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity
            (
                new[]
                {
                    // Id/username
                    new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                    // Jti - for refresh?
                    // Email
                }
            ),
            // TODO: temp
            Expires = DateTime.UtcNow.AddDays(5),
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }
}
