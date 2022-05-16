using OneOf;
using Model = CtServer.Features.Events.ReadModel;

namespace CtServer.Features.Users.Events;

public static class Index
{
    public record Query(int UserId) : IRequest<OneOf<Model[], NotFound>>;

    public class Handler : IRequestHandler<Query, OneOf<Model[], NotFound>>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<OneOf<Model[], NotFound>> Handle(Query request, CancellationToken cancellationToken)
        {
            var models = await _ctx.Users
                .AsNoTracking()
                .Where(x => x.Id == request.UserId)
                .SelectMany(x => x.Events)
                .OrderBy(x => x.Id)
                .Select(Model.FromEntity)
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
