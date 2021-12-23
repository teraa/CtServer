namespace CtServer.Features.Sections;

public static class Index
{
    public record Query : IRequest<Model[]>;

    public record Model
    (
        int Id,
        int EventId,
        int LocationId,
        string Title,
        string[] Chairs,
        DateTimeOffset StartAt,
        DateTimeOffset EndAt,
        int BackgroundColor
    );

    public class Handler : IRequestHandler<Query, Model[]>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<Model[]> Handle(Query request, CancellationToken cancellationToken)
        {
            var models = await _ctx.Sections
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new Model
                (
                    x.Id,
                    x.EventId,
                    x.LocationId,
                    x.Title,
                    x.Chairs,
                    x.StartAt,
                    x.EndAt,
                    x.BackgroundColor
                ))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            return models;
        }
    }
}
