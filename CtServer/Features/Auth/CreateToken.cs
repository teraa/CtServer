using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CtServer.Features.Auth;

public static class CreateToken
{
    public record Command
    (
        Model Model
    ) : IRequest<string>;

    public record Model(string Username);

    // TODO: is this used?
    public class ModelValidator : AbstractValidator<Model>
    {
        public ModelValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
        }
    }

    public record Result(string Token);

    public class Handler : IRequestHandler<Command, string>
    {
        private readonly SymmetricSecurityKey _key;

        public Handler(IConfiguration configuration)
        {
            string secret = configuration["JWT_SECRET"];
            byte[] bytes = Encoding.ASCII.GetBytes(secret);
            _key = new SymmetricSecurityKey(bytes);
        }

        public Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity
                (
                    new[]
                    {
                        // Id/username
                        new Claim(JwtRegisteredClaimNames.Sub, request.Model.Username),
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

            return Task.FromResult(tokenString);
        }
    }
}
