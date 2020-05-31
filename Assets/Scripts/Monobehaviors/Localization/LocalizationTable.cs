using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LocalizationTable
{
    [SerializeField]
    private string name;

    [SerializeField]
    private Dictionary<LocalizationAssetKey, LocalizationAsset<string>> stringAssets = new Dictionary<LocalizationAssetKey, LocalizationAsset<string>>();

    public LocalizationTable(string _name)
    {
        name = _name;
    }

    public void Clear()
    {
        stringAssets.Clear();
    }

    public void AddStringAsset(LocalizationAssetKey key, LocalizationAsset<string> asset)
    {
        stringAssets[key] = asset;
    }

    public LocalizationAsset<string> GetStringAsset(LocalizationAssetKey key)
    {
        if(stringAssets.ContainsKey(key) == false)
        {
            return null;
        }
        return stringAssets[key];
    }
}
