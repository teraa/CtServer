namespace CtServer.Features.Sections;

public static class GetPresentations
{
    public record Query(int SectionId) : IRequest<Model[]?>;

    public record Model
    (
        int Id,
        string Title,
        string[] Authors,
        string Description,
        int Position,
        int DurationMinutes,
        string? Attachment,
        string? MainAuthorPhoto
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

            var models = await ctx.Presentations
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new Model
                (
                    x.Id,
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


            if (!models.Any())
            {
                var exists = await ctx.Sections
                    .AnyAsync(x => x.Id == request.SectionId, cancellationToken)
                    .ConfigureAwait(false);

                if (!exists)
                    return null;
            }

            return models;
        }
    }
}
