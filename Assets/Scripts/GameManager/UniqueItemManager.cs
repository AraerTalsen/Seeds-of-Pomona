using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class UniqueItemManager
{
    private static string _directory = "Assets/Resources/ScriptableObjects/UniqueItems/";

    public static void Save(ScriptableObject item) => BuildAsset.BuildAt(item, _directory, "");

    public static void Delete(ScriptableObject item) => BuildAsset.Destroy(item);
}
