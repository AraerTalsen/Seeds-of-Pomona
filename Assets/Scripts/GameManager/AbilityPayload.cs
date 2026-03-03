using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityPayload : Payload
{
    public AbilityState<BehaviorContext> ability;
    public int probability;
}
