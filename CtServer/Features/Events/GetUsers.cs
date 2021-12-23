namespace CtServer.Features.Events;

public static class GetUsers
{
    public record Query(int EventId) : IRequest<Model[]?>;

    public record Model
    (
        int Id,
        string Username
    );

    public class Handler : IRequestHandler<Query, Model[]?>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<Model[]?> Handle(Query request, CancellationToken cancellationToken)
        {
            var models = await _ctx.UserEvents
                .AsNoTracking()
                .Where(x => x.EventId == request.EventId)
                .Select(x => x.User)
                .OrderBy(x => x.Id)
                .Select(x => new Model
                (
                    x.Id,
                    x.Username
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
