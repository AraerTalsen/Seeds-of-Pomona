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

   [SerializeField][TextArea] protected string toolTip;
   [SerializeField][TextArea] protected string altToolTip;
   public bool UseAltToolTip { get; set; }

   public int id, maxStackSize = 20;
   public Sprite sprite;
   public List<ItemCategory> categories = new() { ItemCategory.Basic };
   public int[] outputItems;
   public int specialOutputID = -1;
   public int specialChance;
   public virtual string CurrentToolTip
   {
      get
      {
         return altToolTip.CompareTo("") != 0 && UseAltToolTip ? altToolTip : toolTip;
      }
   }
}
