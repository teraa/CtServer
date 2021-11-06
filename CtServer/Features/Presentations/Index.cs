using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Presentations
{
    public static class Index
    {
        public record Query : IRequest<Model[]>;

        public record Model
        (
            int Id,
            int SectionId,
            string Title,
            string[] Authors,
            string Description,
            int Position,
            string? Attachment,
            string? MainAuthorPhoto
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

                var models = await ctx.Presentations
                    .AsNoTracking()
                    .Select(x => new Model
                    (
                        x.Id,
                        x.SectionId,
                        x.Title,
                        x.Authors,
                        x.Description,
                        x.Position,
                        x.Attachment,
                        x.MainAuthorPhoto
                    ))
                    .ToArrayAsync(cancellationToken)
                    .ConfigureAwait(false);

                return models;
            }
        }
    }
}
