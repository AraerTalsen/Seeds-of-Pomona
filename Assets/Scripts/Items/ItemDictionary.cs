using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemDictionary
{
    public static Dictionary<int, Item> items = new()
    {
        {0, Resources.Load("ScriptableObjects/Items/Smoke Plant Seeds") as Item},
        {1, Resources.Load("ScriptableObjects/Items/Leap Plant Seeds") as Item},
        {2, Resources.Load("ScriptableObjects/Items/Pop Plant Seeds") as Item},
        {3, Resources.Load("ScriptableObjects/Items/Big Gourd Seeds") as Item},
        {4, Resources.Load("ScriptableObjects/Items/Smoke Flower") as Item},
        {5, Resources.Load("ScriptableObjects/Items/Leap Flower") as Item},
        {6, Resources.Load("ScriptableObjects/Items/Pop Flower") as Item},
        {7, Resources.Load("ScriptableObjects/Items/Gourd Flower") as Item},
        {8, Resources.Load("ScriptableObjects/Effects/Smoke Pod") as Item},
        {9, Resources.Load("ScriptableObjects/Effects/Leap Seed") as Item},
        {10, Resources.Load("ScriptableObjects/Effects/Pop Nut") as Item},
        {11, Resources.Load("ScriptableObjects/Effects/Bash Bulb") as Item},
        {12, Resources.Load("ScriptableObjects/Items/Bio Mass") as Item},
        {13, Resources.Load("ScriptableObjects/Items/Med Pack") as Item},
        {14, Resources.Load("ScriptableObjects/Items/Escape Transport") as Item},
    };
}
