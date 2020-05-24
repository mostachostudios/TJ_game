using System;
using UnityEngine;

[Serializable]
public class LocalizationAsset<T> where T : class
{
    [SerializeField]
    protected LocalizationAssetKey key;

    [SerializeField]
    protected T[] content = new T[0];

    public LocalizationAsset(LocalizationAssetKey _key, int numOfLanguages)
    {
        key = _key;
        content = new T[numOfLanguages];
    }

    public LocalizationAssetKey GetKey() { return key; }

    public T GetValue(LocalizationLanguageKey language)
    {
        int index = ((int)language);
        if (index < content.Length)
        {
            return content[index];
        }
        return null;
    }

    public void AddValue(T value, int index)
    {
        if (index < content.Length)
        {
            content[index] = value;
        }
    }

}
