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
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<Model[]?> Handle(Query request, CancellationToken cancellationToken)
        {
            var models = await _ctx.Sections
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
                var exists = await _ctx.Events
                    .AnyAsync(x => x.Id == request.EventId, cancellationToken)
                    .ConfigureAwait(false);

                if (!exists)
                    return null;
            }

            return models;
        }
    }
}
