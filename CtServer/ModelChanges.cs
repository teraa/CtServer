namespace CtServer;

public class ModelChanges
{
    private readonly Dictionary<string, Entry> _changes;

    public ModelChanges()
    {
        _changes = new();
    }

    public IReadOnlyDictionary<string, Entry> Map => _changes;

    public bool Set<T>(string name, T current, T next, Action<T> set)
        where T : IEquatable<T>
    {
        if (current.Equals(next))
            return false;

        _changes[name] = new(current, next);

        set(next);

        return true;
    }

    public bool Set<T>(string name, T[] current, T[] next, Action<T[]> set)
        where T : IEquatable<T>
    {
        if (current.SequenceEqual(next))
            return false;

        _changes[name] = new(current, next);

        set(next);

        return true;
    }

    public record Entry(object Old, object New);
}
