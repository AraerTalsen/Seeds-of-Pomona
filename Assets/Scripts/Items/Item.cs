using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
   public int id, maxStackSize = 20;
   public Sprite sprite;
   public int[] usableInMachines;
   public int[] outputItems;
}
