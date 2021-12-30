namespace CtServer.Features.Users.Events;

public static class Index
{
    public record Query(int UserId) : IRequest<Model[]?>;

    public record Model
    (
        int Id,
        string Title,
        string Description,
        DateTimeOffset StartAt,
        DateTimeOffset EndAt
    );

    public class Handler : IRequestHandler<Query, Model[]?>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<Model[]?> Handle(Query request, CancellationToken cancellationToken)
        {
            var models = await _ctx.Users
                .AsNoTracking()
                .Where(x => x.Id == request.UserId)
                .SelectMany(x => x.Events)
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

            if (!models.Any())
            {
                var exists = await _ctx.Users
                    .AnyAsync(x => x.Id == request.UserId, cancellationToken)
                    .ConfigureAwait(false);

                if (!exists)
                    return null;
            }

            return models;
        }
    }
}
