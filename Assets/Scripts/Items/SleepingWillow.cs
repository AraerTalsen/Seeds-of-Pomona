using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Sleeping Willow")]
public class SleepingWillow : Tool
{
    public override void UseAbility(GameObject user)
    {
        Debug.Log("Whipped");
    }
}
