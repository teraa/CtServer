using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Events
{
    public static class Index
    {
        public record Query : IRequest<Model[]>;

        public record Model
        (
            int Id,
            string Title,
            DateTimeOffset StartAt,
            DateTimeOffset EndAt,
            int SectionCount
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

                var models = await ctx.Events
                    .AsNoTracking()
                    .Select(x => new Model
                    (
                        x.Id,
                        x.Title,
                        x.StartAt,
                        x.EndAt,
                        x.Sections.Count()
                    ))
                    .ToArrayAsync(cancellationToken)
                    .ConfigureAwait(false);

                return models;
            }
        }
    }
}