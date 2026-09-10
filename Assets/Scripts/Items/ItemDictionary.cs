using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemDictionary
{
    public static Dictionary<int, Item> items = new()
    {
        {0, Resources.Load("ScriptableObjects/Items/Seeds/Smoke Plant Seeds") as Item},
        {1, Resources.Load("ScriptableObjects/Items/Seeds/Leap Plant Seeds") as Item},
        {2, Resources.Load("ScriptableObjects/Items/Seeds/Pop Plant Seeds") as Item},
        {3, Resources.Load("ScriptableObjects/Items/Seeds/Big Gourd Seeds") as Item},
        {4, Resources.Load("ScriptableObjects/Items/Smoke Flower") as Item},
        {5, Resources.Load("ScriptableObjects/Items/Leap Flower") as Item},
        {6, Resources.Load("ScriptableObjects/Items/Pop Flower") as Item},
        {7, Resources.Load("ScriptableObjects/Items/Gourd Flower") as Item},
        {8, Resources.Load("ScriptableObjects/Items/PowerUps/Smoke Pod") as Item},
        {9, Resources.Load("ScriptableObjects/Items/PowerUps/Leap Seed") as Item},
        {10, Resources.Load("ScriptableObjects/Items/PowerUps/Pop Nut") as Item},
        {11, Resources.Load("ScriptableObjects/Items/PowerUps/Bash Bulb") as Item},
        {12, Resources.Load("ScriptableObjects/Items/Bio Mass") as Item},
        {13, Resources.Load("ScriptableObjects/Items/Tools/Med Pack") as Item},
        {14, Resources.Load("ScriptableObjects/Items/Tools/Escape Transport") as Item},
    };
}
