namespace CtServer.Features.Presentations;

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
        int DurationMinutes,
        string? Attachment,
        string? MainAuthorPhoto
    );

    public class Handler : IRequestHandler<Query, Model[]>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<Model[]> Handle(Query request, CancellationToken cancellationToken)
        {
            var models = await _ctx.Presentations
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new Model
                (
                    x.Id,
                    x.SectionId,
                    x.Title,
                    x.Authors,
                    x.Description,
                    x.Position,
                    (int)x.Duration.TotalMinutes,
                    x.Attachment,
                    x.MainAuthorPhoto
                ))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            return models;
        }
    }
}
