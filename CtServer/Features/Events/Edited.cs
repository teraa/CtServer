using CtServer.Services;
using WebPush;

namespace CtServer.Features.Events;

public static class Edited
{
    public record Notification
    (
        Model Model
    ) : INotification;

    public record Model
    (
        int Id
    );

    public class Handler : INotificationHandler<Notification>
    {
        private readonly CtDbContext _ctx;
        private readonly NotificationService _service;

        public Handler(CtDbContext ctx, NotificationService service)
        {
            _ctx = ctx;
            _service = service;
        }

        public async Task Handle(Notification notification, CancellationToken cancellationToken)
        {
            var subscriptions = await _ctx.UserEvents
                .AsNoTracking()
                .Where(x => x.EventId == notification.Model.Id)
                .SelectMany(x => x.User.Subscriptions)
                .Select(x => new PushSubscription
                {
                    Endpoint = x.Endpoint,
                    P256DH = x.P256dh,
                    Auth = x.Auth
                })
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            await _service.SendAsync(subscriptions, $"Event {notification.Model.Id} edited.", cancellationToken)
                    .ConfigureAwait(false);
        }
    }
}
