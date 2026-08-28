using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class UniqueItemManager
{
    public static void Save(string key, PUp item) => AssetSaves.Save(item, key);

    public static PUp Load(string key) => AssetSaves.Load<PUp>(key);
    public static void Delete(string key) => AssetSaves.Delete(key);
}
