namespace ALRC.Converters;

internal static class Dictionary
{
    public static TValue? GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue?> dictionary, TKey key, TValue? defaultValue = default) where TKey : notnull
    {
        TValue? value;
        if (dictionary.TryGetValue(key, out value!))
        {
            return value;
        }
        return defaultValue;
    }
}