using System.Text.Json;
using CtServer.Data.Models;
using CtServer.Options;
using Microsoft.Extensions.Options;
using WebPush;

namespace CtServer.Services;

public class NotificationService : IDisposable
{
    private readonly ILogger<NotificationService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly WebPushClient? _client;

    public NotificationService(ILogger<NotificationService> logger, IOptions<WebPushOptions> options, JsonSerializerOptions jsonOptions)
    {
        _logger = logger;
        _jsonOptions = jsonOptions;

        var opt = options.Value;
        if (!opt.IsEnabled) return;

        _client = new WebPushClient();
        _client.SetGcmApiKey(opt.ServerKey);
        _client.SetVapidDetails(new VapidDetails
        {
            Subject = opt.Subject,
            PublicKey = opt.PublicKey,
            PrivateKey = opt.PrivateKey,
        });

        _logger.LogDebug("Enabled");
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    public async Task SendAsync(PushSubscription subscription, string? payload = null, CancellationToken cancellationToken = default)
    {
        if (_client is null) return;

        try
        {
            _logger.LogDebug("Sending notification to: {0}", subscription.Endpoint);

            await _client.SendNotificationAsync(subscription, payload, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification");
        }
    }

    public async Task SendAsync(IEnumerable<PushSubscription> subscriptions, string? payload = null, CancellationToken cancellationToken = default)
    {
        if (_client is null) return;

        foreach (var sub in subscriptions)
            await SendAsync(sub, payload, cancellationToken)
                .ConfigureAwait(false);
    }

    public async Task SendAsync(IEnumerable<PushSubscription> subscriptions, Payload payload, CancellationToken cancellationToken)
    {
        string payloadJson = JsonSerializer.Serialize(payload, _jsonOptions);
        await SendAsync(subscriptions, payloadJson, cancellationToken).ConfigureAwait(false);
    }

    public record Payload
    (
        int EventId,
        string EventTitle,
        NotificationType Type
    );
}
