using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableKeyValuePair<TKey, TValue>
{
    public TKey Key;
    public TValue Value;

    public SerializableKeyValuePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}

[Serializable]
public class SerializableDictionary<TKey, TValue>
{
    public List<SerializableKeyValuePair<TKey, TValue>> items = new List<SerializableKeyValuePair<TKey, TValue>>();

    public SerializableDictionary(Dictionary<TKey, TValue> dict)
    {
        foreach (var kvp in dict)
        {
            items.Add(new SerializableKeyValuePair<TKey, TValue>(kvp.Key, kvp.Value));
        }
    }

    public Dictionary<TKey, TValue> ToDictionary()
    {
        Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
        foreach (var kvp in items)
        {
            dict[kvp.Key] = kvp.Value;
        }
        return dict;
    }
}