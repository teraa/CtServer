using System.Text.Json;
using CtServer.Data.Models;
using CtServer.Services;
using WebPush;

namespace CtServer.Features.Notifications;

public static class Push
{
    public record Notification
    (
        int EventId,
        string EventTitle,
        NotificationType Type,
        object Data
    ) : INotification;

    public class Handler<TData> : INotificationHandler<Notification>
    {
        private readonly CtDbContext _ctx;
        private readonly NotificationService _service;
        private readonly JsonSerializerOptions _jsonOptions;

        public Handler(
            CtDbContext ctx,
            NotificationService service,
            JsonSerializerOptions jsonOptions)
        {
            _ctx = ctx;
            _service = service;
            _jsonOptions = jsonOptions;
        }

        public async Task Handle(Notification notification, CancellationToken cancellationToken)
        {
            string dataJson = JsonSerializer.Serialize(notification.Data, _jsonOptions);

            var entity = new CtServer.Data.Models.Notification
            {
                EventId = notification.EventId,
                Type = notification.Type,
                Data = dataJson,
                CreatedAt = DateTimeOffset.UtcNow,
            };

            _ctx.Add(entity);
            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var subscriptions = await _ctx.UserEvents
                .AsNoTracking()
                .Where(x => x.EventId == notification.EventId)
                .SelectMany(x => x.User.Subscriptions)
                .Select(x => new PushSubscription
                {
                    Endpoint = x.Endpoint,
                    P256DH = x.P256dh,
                    Auth = x.Auth
                })
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            NotificationService.Payload payload = new(notification.EventId, notification.EventTitle, notification.Type);

            await _service.SendAsync(subscriptions, payload, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
