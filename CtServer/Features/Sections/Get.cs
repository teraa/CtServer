using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Sections
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
            string Location,
            string[] Chairs,
            DateTimeOffset StartAt,
            DateTimeOffset EndAt,
            int BackgroundColor,
            IReadOnlyList<Model.Presentation> Presentations
        )
        {
            public record Presentation
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
        }

        public class Handler : IRequestHandler<Query, Model?>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Handler(IServiceScopeFactory scopeFactory)
                => _scopeFactory = scopeFactory;

            public async Task<Model?> Handle(Query request, CancellationToken cancellationToken)
            {
                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

                var model = await ctx.Sections
                    .AsNoTracking()
                    .AsSingleQuery()
                    .Where(x => x.Id == request.Id)
                    .Select(x => new Model
                    (
                        x.Id,
                        x.Title,
                        x.Location,
                        x.Chairs,
                        x.StartAt,
                        x.EndAt,
                        x.BackgroundColor,
                        x.Presentations.Select(x => new Model.Presentation
                        (
                            x.Id,
                            x.Title,
                            x.Authors,
                            x.Description,
                            x.StartAt,
                            x.EndAt,
                            x.Attachment,
                            x.MainAuthorPhoto
                        )).ToArray()
                    ))
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);

                return model;
            }
        }
    }
}
