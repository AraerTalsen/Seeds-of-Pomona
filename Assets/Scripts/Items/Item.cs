using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
[System.Serializable]
public class Item : ScriptableObject
{
   [System.Serializable]
   public struct SpecialOutput
   {
      [SerializeField] private int itemId;
      [SerializeField] private int spawnWeight;

      public int ItemID => itemId;
      public int SpawnWeight => spawnWeight;
      public void Deconstruct(out int id, out int weight)
      {
         id = itemId;
         weight = spawnWeight;
      }
   }
   public enum ItemCategory
   {
      Seed,
      Deconstructable,
      Tool,
      Basic
   }
   [SerializeField][TextArea] private string toolTip;
   [SerializeField][TextArea] private string altToolTip;
   public bool UseAltToolTip { get; set; }
   public string CurrentToolTip => GetToolTip();

   public int id, maxStackSize = 20;
   public Sprite sprite;
   public List<ItemCategory> categories = new() { ItemCategory.Basic };
   public int[] outputItems;
   public int specialSpawnChance = 0;
   public List<SpecialOutput> specialOutputs = new();

   protected virtual string GetToolTip() => string.IsNullOrEmpty(altToolTip) && ! UseAltToolTip ? toolTip : altToolTip;

   public int SelectSpecialItem()
   {
      if(specialSpawnChance > 0) 
      {
         int weightedTotal = specialOutputs.Sum( e => e.SpawnWeight);
         int rand = Random.Range(0, weightedTotal);
         int accruedWeight = 0;

         foreach((int id, int weight) in specialOutputs)
         {
            accruedWeight += weight;
            if(rand < accruedWeight)
            {
               Debug.Log(rand);
               return id;
            }
         }
      }
      return -1;
   }
}
