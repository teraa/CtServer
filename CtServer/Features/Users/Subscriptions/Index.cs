using OneOf;

namespace CtServer.Features.Users.Subscriptions;

public static class Index
{
    public record Query(int UserId) : IRequest<OneOf<Model[], NotFound>>;

    public record Model
    (
        string Endpoint
    );

    public class Handler : IRequestHandler<Query, OneOf<Model[], NotFound>>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<OneOf<Model[], NotFound>> Handle(Query request, CancellationToken cancellationToken)
        {
            var models = await _ctx.Subscriptions
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId)
                .OrderBy(x => x.Id)
                .Select(x => new Model
                (
                    x.Endpoint
                ))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!models.Any())
            {
                var exists = await _ctx.Users
                    .AnyAsync(x => x.Id == request.UserId, cancellationToken)
                    .ConfigureAwait(false);

                if (!exists)
                    return new NotFound();
            }

            return models;
        }
    }
}
