using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/GardenPlotData")]
public class GardenPlotData : PersistentDataBase
{    
    [SerializeField]
    private List<int> itemIds;
    [SerializeField]
    private List<int> plotIndeces;
    [SerializeField]
    private List<int> growthStages;
    [SerializeField]
    private List<float> growthProgress;
    [SerializeField]
    private float unloadTime;
    [SerializeField]
    private bool isPersisting = false;

    public List<int> ItemIds {get => itemIds; set => itemIds = value;}
    public List<int> PlotIndeces {get => plotIndeces; set => plotIndeces = value;}
    public List<int> GrowthStages {get => growthStages; set => growthStages = value;}
    public List<float> GrowthProgress {get => growthProgress; set => growthProgress = value;}
    public float UnloadTime {get => unloadTime; set => unloadTime = value;}
    public bool IsPersisting {get => isPersisting; set => isPersisting = value;}
}
