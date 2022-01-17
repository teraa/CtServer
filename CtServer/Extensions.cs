using System.Security.Claims;

namespace CtServer;

public static class Extensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.NameIdentifier);

    public static bool HasUserId(this ClaimsPrincipal principal, string id)
        => principal.HasClaim(ClaimTypes.NameIdentifier, id);


    public static IConfigurationSection GetOptionsSection<TOptions>(this IConfiguration configuration)
    {
        const string suffix = "Options";

        string name = typeof(TOptions).Name;

        if (name.EndsWith(suffix))
            name = name[..^suffix.Length];

        return configuration.GetRequiredSection(name);
    }

    public static TOptions GetOptions<TOptions>(this IConfiguration configuration)
    {
        return configuration.GetOptionsSection<TOptions>().Get<TOptions>();
    }

    public static IServiceCollection AddOptionsWithSection<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class
    {
        return services.Configure<TOptions>(configuration.GetOptionsSection<TOptions>());
    }

    public static bool TryUpdate<T>(T currentValue, T value, Action<T> set)
        where T : IEquatable<T>
    {
        if (currentValue.Equals(value))
            return false;

        set(value);
        return true;
    }

    public static bool TryUpdate<T>(T[] currentValues, T[] values, Action<T[]> set)
    {
        if (currentValues.SequenceEqual(values))
            return false;

        set(values);
        return true;
    }
}
