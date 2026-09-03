using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/GeneEditorData")]
public class GeneEditorData : InventoryData
{
    [SerializeField]
    private float unloadTime;
    [SerializeField]
    private float currentProgress;
    [SerializeField] private List<InventoryEntry> input;
    [SerializeField] private List<InventoryEntry> failOutput, successOutput;

    public float UnloadTime { get => unloadTime; set => unloadTime = value; }
    public float CurrentProgress { get => currentProgress; set => currentProgress = value; }
    public List<InventoryEntry> Input { get => input; set => input = value; }
    public List<InventoryEntry> FailOutput { get => failOutput; set => failOutput = value; }
    public List<InventoryEntry> SuccessOutput { get => successOutput; set => successOutput = value; }
}
