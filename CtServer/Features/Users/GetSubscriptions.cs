namespace CtServer.Features.Users;

public static class GetSubscriptions
{
    public record Query(int UserId) : IRequest<Model[]?>;

    public record Model
    (
        int Id
    );

    public class Handler : IRequestHandler<Query, Model[]?>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<Model[]?> Handle(Query request, CancellationToken cancellationToken)
        {
            var models = await _ctx.Subscriptions
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId)
                .OrderBy(x => x.Id)
                .Select(x => new Model
                (
                    x.Id
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
