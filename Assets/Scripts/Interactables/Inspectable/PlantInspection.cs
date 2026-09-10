using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlantInspection : Inspectable
{
    private BoxCollider2D col;

    public string PlantName { get; set; }
    public override bool IsInspectable
    {
        get => base.IsInspectable;
        set
        {
            base.IsInspectable = value;
            if (col == null)
            {
                col = GetComponent<BoxCollider2D>();
            }

            col.enabled = base.IsInspectable;
        }
    }
    public int PlantGrowthStage
    {
        set
        {
            UpdateMessage(value);
        }
    }

    private void UpdateMessage(int growthStage)
    {
        message = $"The {PlantName}'s current growth stage is {growthStage} of 4.";
    }
}
