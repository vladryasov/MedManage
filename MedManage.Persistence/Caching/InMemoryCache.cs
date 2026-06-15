using System.Collections.Concurrent;

namespace MedManage.Persistence.Caching;

public class InMemoryCache : IInMemoryCache
{
    private readonly ConcurrentDictionary<string, CacheEntry> _entries = new();

    public bool TryGet<T>(string key, out T? value)
    {
        if (!_entries.TryGetValue(key, out var entry) || entry.IsExpired)
        {
            if (entry is not null)
            {
                _entries.TryRemove(key, out _);
            }

            value = default;
            return false;
        }

        value = (T)entry.Value;
        return true;
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        DateTime? expiresAt = expiration.HasValue
            ? DateTime.UtcNow.Add(expiration.Value)
            : null;

        _entries[key] = new CacheEntry(value!, expiresAt);
    }

    public bool Remove(string key)
    {
        return _entries.TryRemove(key, out _);
    }

    public bool ContainsKey(string key)
    {
        if (!_entries.TryGetValue(key, out var entry))
        {
            return false;
        }

        if (entry.IsExpired)
        {
            _entries.TryRemove(key, out _);
            return false;
        }

        return true;
    }

    public void RemoveByPrefix(string prefix)
    {
        var keysToRemove = _entries.Keys.Where(k => k.StartsWith(prefix)).ToList();
        foreach (var key in keysToRemove)
            _entries.TryRemove(key, out _);
    }

    public void Clear()
    {
        _entries.Clear();
    }

    private sealed class CacheEntry
    {
        public CacheEntry(object value, DateTime? expiresAt)
        {
            Value = value;
            ExpiresAt = expiresAt;
        }

        public object Value { get; }

        public DateTime? ExpiresAt { get; }

        public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
    }
}
