using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Presentations
{
    public static class Get
    {
        public record Query
        (
            int Id
        ) : IRequest<Model?>;

        public record Model
        (
            int Id,
            string Title,
            string[] Authors,
            string Description,
            DateTimeOffset StartAt,
            DateTimeOffset EndAt,
            string? Attachment,
            string? MainAuthorPhoto
        );

        public class Handler : IRequestHandler<Query, Model?>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Handler(IServiceScopeFactory scopeFactory)
                => _scopeFactory = scopeFactory;

            public async Task<Model?> Handle(Query request, CancellationToken cancellationToken)
            {
                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

                var model = await ctx.Presentations
                    .AsNoTracking()
                    .AsSingleQuery()
                    .Where(x => x.Id == request.Id)
                    .Select(x => new Model
                    (
                        x.Id,
                        x.Title,
                        x.Authors,
                        x.Description,
                        x.StartAt,
                        x.EndAt,
                        x.Attachment,
                        x.MainAuthorPhoto
                    ))
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);

                return model;
            }
        }
    }
}