using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class BuildAsset
{
    public static void BuildAt(ScriptableObject asset, string directory, string folderName = "")
    {
        string newFolderName = folderName.CompareTo("") == 0 ? asset.name : folderName;
        string folderPath = directory + newFolderName;

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder(directory.TrimEnd('/'), newFolderName);
        }
        
        string path = $"{folderPath}/{asset.name}.asset";
        path = AssetDatabase.GenerateUniqueAssetPath(path);
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
    }

    public static bool ContainsType(System.Type type, string[] checkFolders) =>
        AssetDatabase.FindAssets($"t:{type}", checkFolders).Length > 0;

    public static int CountType(System.Type type, string[] checkFolders) =>
        AssetDatabase.FindAssets($"t:{type}", checkFolders).Length;

    public static void Destroy(ScriptableObject asset, string clonePath)
    {
        string path = AssetDatabase.GetAssetPath(asset);
        if(path.StartsWith(clonePath, System.StringComparison.Ordinal))
            AssetDatabase.DeleteAsset(path);
        else
            Debug.Log("Canceled attempt to delete item outside of clone folder");
    }
}
