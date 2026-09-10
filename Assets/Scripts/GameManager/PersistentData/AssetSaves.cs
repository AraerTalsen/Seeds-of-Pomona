using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AssetSaves
{
    public static void Save<T>(T asset, string key)
    {
        PlayerPrefs.SetString(key, JsonUtility.ToJson(asset));
        PlayerPrefs.Save();
    }

    public static T Load<T>(string key) where T : PowerUp
    {
        if(!PlayerPrefs.HasKey(key))
        {
            return null;
        }
        else
        {
            T instance = ScriptableObject.CreateInstance<T>();
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), instance);
            return instance;
        }
    }

    public static void Delete(string key) => PlayerPrefs.DeleteKey(key);
}
