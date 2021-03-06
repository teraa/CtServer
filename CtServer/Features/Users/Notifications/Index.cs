using CtServer.Data.Models;

namespace CtServer.Features.Users.Notifications;

public static class Index
{
    public record Query(int UserId) : IRequest<Model[]?>;

    public record Model
    (
        int EventId,
        string EventTitle,
        DateTimeOffset CreatedAt,
        NotificationType Type,
        object Data
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
                .Where(x => x.UserId == request.UserId)
                .SelectMany(x => x.Event.Notifications)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new Model
                (
                    x.Event.Id,
                    x.Event.Title,
                    x.CreatedAt,
                    x.Type,
                    x.Data
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
