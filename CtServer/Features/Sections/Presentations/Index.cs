using Model = CtServer.Features.Presentations.ReadModel;

namespace CtServer.Features.Sections.Presentations;

public static class Index
{
    public record Query(int SectionId) : IRequest<Model[]?>;

    public class Handler : IRequestHandler<Query, Model[]?>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<Model[]?> Handle(Query request, CancellationToken cancellationToken)
        {
            var models = await _ctx.Presentations
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(Model.FromEntity)
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!models.Any())
            {
                var exists = await _ctx.Sections
                    .AnyAsync(x => x.Id == request.SectionId, cancellationToken)
                    .ConfigureAwait(false);

                if (!exists)
                    return null;
            }

            return models;
        }
    }
}
