using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Events
{
    public static class Delete
    {
        public record Command(int Id) : IRequest<bool>;

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Handler(IServiceScopeFactory scopeFactory)
                => _scopeFactory = scopeFactory;

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

                var entity = await ctx.Events
                    .AsQueryable()
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (entity is null) return false;

                ctx.Events.Remove(entity);

                await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return true;
            }
        }
    }
}