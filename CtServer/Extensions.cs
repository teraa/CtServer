using System.Security.Claims;

namespace CtServer;

public static class Extensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.NameIdentifier);

    public static bool HasUserId(this ClaimsPrincipal principal, string id)
        => principal.HasClaim(ClaimTypes.NameIdentifier, id);
}
