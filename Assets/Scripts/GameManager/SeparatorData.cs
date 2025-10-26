using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is an unnecessary extra class, what is a better, more concise way to include this into other
//data storage scripts?
[CreateAssetMenu(menuName = "Scriptable Objects/SeparatorData")]
public class SeparatorData : InventoryData
{
    [SerializeField]
    private float unloadTime;
    [SerializeField]
    private float currentProgress;

    public float UnloadTime { get => unloadTime; set => unloadTime = value; }
    public float CurrentProgress { get => currentProgress; set => currentProgress = value; }
}
