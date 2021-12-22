using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CtServer.Services;

public class TokenService
{
    public const string SecretName = "JWT_SECRET";

    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration configuration)
    {
        string secret = configuration[SecretName];
        byte[] bytes = Encoding.ASCII.GetBytes(secret);
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
