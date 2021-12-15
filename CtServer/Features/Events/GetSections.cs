using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Events;

public static class GetSections
{
    public record Query(int EventId) : IRequest<Model[]?>;

    public record Model
    (
        int Id,
        int LocationId,
        string Title,
        string[] Chairs,
        DateTimeOffset StartAt,
        DateTimeOffset EndAt,
        int BackgroundColor
    );

    public class Handler : IRequestHandler<Query, Model[]?>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public Handler(IServiceScopeFactory scopeFactory)
            => _scopeFactory = scopeFactory;

        public async Task<Model[]?> Handle(Query request, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

            var models = await ctx.Sections
                .AsNoTracking()
                .Where(x => x.EventId == request.EventId)
                .OrderBy(x => x.Id)
                .Select(x => new Model
                (
                    x.Id,
                    x.LocationId,
                    x.Title,
                    x.Chairs,
                    x.StartAt,
                    x.EndAt,
                    x.BackgroundColor
                ))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!models.Any())
            {
                var exists = await ctx.Events
                    .AnyAsync(x => x.Id == request.EventId, cancellationToken)
                    .ConfigureAwait(false);

                if (!exists)
                    return null;
            }

            return models;
        }
    }
}
