using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Evolutions")]
public class StatEvolve : Evolutions
{
    [SerializeField]
    private List<Stats> latentStats;

    [SerializeField]
    private List<int> statMods;
}
