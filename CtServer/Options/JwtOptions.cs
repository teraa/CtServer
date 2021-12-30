#pragma warning disable CS8618
namespace CtServer.Options;

public class JwtOptions
{
    public const string Section = "Jwt";

    public string Secret { get; init; }
}
