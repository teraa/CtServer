#pragma warning disable CS8618
namespace CtServer.Options;

public class WebPushOptions
{
    public bool IsEnabled { get; init; }
    public string ServerKey { get; init; }
    public string SenderId { get; init; }
    public string Subject { get; init; }
    public string PublicKey { get; init; }
    public string PrivateKey { get; init; }
}
