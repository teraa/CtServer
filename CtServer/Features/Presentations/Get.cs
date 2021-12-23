namespace CtServer.Features.Presentations;

public static class Get
{
    public record Query
    (
        int Id
    ) : IRequest<Model?>;

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

    public class Handler : IRequestHandler<Query, Model?>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<Model?> Handle(Query request, CancellationToken cancellationToken)
        {
            var model = await _ctx.Presentations
                .AsNoTracking()
                .Where(x => x.Id == request.Id)
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
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return model;
        }
    }
}
