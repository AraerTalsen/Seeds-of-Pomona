using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class UniqueItemManager
{
    public static void Save(string key, PowerUp item) => AssetSaves.Save(item, key);

    public static PowerUp Load(string key) => AssetSaves.Load<PowerUp>(key);
    public static void Delete(string key) => AssetSaves.Delete(key);
}
