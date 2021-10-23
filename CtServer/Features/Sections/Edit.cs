using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Sections
{
    public static class Edit
    {
        public record Command
        (
            int Id,
            Model Model
        ) : IRequest<bool>;
        public record Model
        (
            string Title,
            string Location,
            string[] Chairs,
            DateTimeOffset StartAt,
            DateTimeOffset EndAt,
            int BackgroundColor
        );

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Handler(IServiceScopeFactory scopeFactory)
                => _scopeFactory = scopeFactory;

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

                var entity = await ctx.Sections
                    .AsQueryable()
                    .Where(x => x.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (entity is null) return false;

                entity.Title = request.Model.Title;
                entity.Location = request.Model.Location;
                entity.Chairs = request.Model.Chairs;
                entity.StartAt = request.Model.StartAt;
                entity.EndAt = request.Model.EndAt;
                entity.BackgroundColor = request.Model.BackgroundColor;

                await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return true;
            }
        }
    }
}
