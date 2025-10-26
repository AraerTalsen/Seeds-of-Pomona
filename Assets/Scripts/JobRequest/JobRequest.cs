using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/JobRequest")]
[System.Serializable]
public class JobRequest : ScriptableObject
{
    public string description;
    public Item requestedItem;
    public int baseItemQty;
    public int qtyVariability = 0;
    public int baseReward;
    public int rewardVariability = 0;
    public float postedTime;
    public int timeline;

    private int chosenItemQty = -1;
    private int chosenReward = -1;

    public int ChosenItemQty
    {
        get
        {
            if(chosenItemQty == -1)
            {
                chosenItemQty = Random.Range(baseItemQty - qtyVariability, baseItemQty + qtyVariability + 1);
            }
            return chosenItemQty;
        }
    }

    public int ChosenReward
    {
        get
        {
            if(chosenReward == -1)
            {
                chosenReward = Random.Range(baseReward - rewardVariability, baseReward + rewardVariability + 1);
                int hundreds = chosenReward / 100;
                int tens = chosenReward % 100 / 10;
                chosenReward = tens < 5 ? hundreds * 100 : (hundreds + 1) * 100;
            }
            return chosenReward;
        }
    }

    public int Deadline
    {
        get
        {
            return (int)postedTime + timeline;
        }
    }
}
