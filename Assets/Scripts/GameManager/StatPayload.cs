using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatPayload : Payload
{
    public List<Stats> stats;
    public List<int> mods;
}
