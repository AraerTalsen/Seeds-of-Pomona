using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemDictionary
{
    public static Dictionary<int, Item> items = new()
    {
        {0, Resources.Load("ScriptableObjects/Items/Seeds") as Item},
        {1, Resources.Load("ScriptableObjects/Items/Flower") as Item},
        {2, Resources.Load("ScriptableObjects/Items/Bio Mass") as Item},
        {3, Resources.Load("ScriptableObjects/Items/Smoke Pod") as Item},
        {4, Resources.Load("ScriptableObjects/Items/Leap Seed") as Item}
    };
}
