using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Screech")]
public class Screech : AbilityState
{
    public override void PerformAction()
    {
        Debug.Log("Screech");
    }
}
