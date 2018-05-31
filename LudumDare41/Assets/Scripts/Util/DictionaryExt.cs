using System.Collections.Generic;

public static class DictionaryExt
{
	public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
	{
		return dict.GetOrDefault(key, default(TValue));
	}

	public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey,TValue> dict, TKey key, TValue defaultValue)
	{
		TValue v;
		if (!dict.TryGetValue(key, out v))
		{
			v = default(TValue);
		}
		return v;
	}
}
