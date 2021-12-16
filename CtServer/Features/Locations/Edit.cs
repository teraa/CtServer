using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Locations;

public static class Edit
{
    public record Command
    (
        int Id,
        Model Model
    ) : IRequest<Response?>;

    public record Model
    (
        int EventId,
        string Name
    );

    public class ModelValidator : AbstractValidator<Model>
    {
        public ModelValidator()
        {
            RuleFor(x => x.EventId).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public record Response;

    public class Handler : IRequestHandler<Command, Response?>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public Handler(IServiceScopeFactory scopeFactory)
            => _scopeFactory = scopeFactory;

        public async Task<Response?> Handle(Command request, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

            var entity = await ctx.Locations
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return null;

            entity.EventId = request.Model.EventId;
            entity.Name = request.Model.Name;

            await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new();
        }
    }
}
