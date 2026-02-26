using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
[System.Serializable]
public class Item : ScriptableObject
{
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
   public int specialOutputID = -1;
   public int specialChance;

   protected virtual string GetToolTip() => string.IsNullOrEmpty(altToolTip) && ! UseAltToolTip ? toolTip : altToolTip;
}
