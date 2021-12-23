namespace CtServer.Features.Events;

public static class Index
{
    public record Query : IRequest<Model[]>;

    public record Model
    (
        int Id,
        string Title,
        string Description,
        DateTimeOffset StartAt,
        DateTimeOffset EndAt
    );

    public class Handler : IRequestHandler<Query, Model[]>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<Model[]> Handle(Query request, CancellationToken cancellationToken)
        {
            var models = await _ctx.Events
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new Model
                (
                    x.Id,
                    x.Title,
                    x.Description,
                    x.StartAt,
                    x.EndAt
                ))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            return models;
        }
    }
}
