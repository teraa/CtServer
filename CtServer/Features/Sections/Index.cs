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
    public static class Index
    {
        public record Query(int EventId) : IRequest<Model[]>;

        public record Model
        (
            int Id,
            string Title,
            string Location,
            string[] Chairs,
            DateTimeOffset StartAt,
            DateTimeOffset EndAt,
            int BackgroundColor,
            int PresentationCount
        );

        public class Handler : IRequestHandler<Query, Model[]>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Handler(IServiceScopeFactory scopeFactory)
                => _scopeFactory = scopeFactory;

            public async Task<Model[]> Handle(Query request, CancellationToken cancellationToken)
            {
                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

                var models = await ctx.Sections
                    .AsNoTracking()
                    .Where(x => x.EventId == request.EventId)
                    .Select(x => new Model
                    (
                        x.Id,
                        x.Title,
                        x.Location,
                        x.Chairs,
                        x.StartAt,
                        x.EndAt,
                        x.BackgroundColor,
                        x.Presentations.Count()
                    ))
                    .ToArrayAsync(cancellationToken)
                    .ConfigureAwait(false);

                return models;
            }
        }
    }
}
