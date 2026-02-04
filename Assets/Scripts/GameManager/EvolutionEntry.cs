using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EvolutionEntry
{
    public int threshold;
    public EvolutionEffect effect;
    [SerializeReference] public Payload payload;
}
