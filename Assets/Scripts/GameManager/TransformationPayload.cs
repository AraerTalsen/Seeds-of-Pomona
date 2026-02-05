using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransformationPayload : Payload
{
    public StatPayload statPayload;
    public AbilityPayload abilityPayload;
    public int scaleIncrease;
}
