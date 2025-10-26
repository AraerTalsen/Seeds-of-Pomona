using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Seeds")]
public class Seeds : Item
{
    public Sprite[] growthStages;
    public int growthRate;
    public int growthOdds;
}
