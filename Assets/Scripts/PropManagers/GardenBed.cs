using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GardenBed : MonoBehaviour
{
    public GardenPlotData persist;
    private List<GardenPlot> plots;

    private void Start()
    {
        persist = RetrievePersistentData();
        FetchGardenPlots();
        
        if (!persist.IsPersisting)
        {
            persist.IsPersisting = true;
        }
        else
        {
            PullData();
        }
    }

    protected GardenPlotData RetrievePersistentData()
    {
        GardenPlotData temp = (GardenPlotData)PersistentData.RetrieveDataContainer(persist.GetType().ToString());
        if(temp == null)
        {
            PersistentData.AddDataContainer(persist);
            return persist;
        }
        else
        {
            return temp;
        }
    }

    private void FetchGardenPlots()
    {
        plots = new();
        Transform[] transforms = transform.Cast<Transform>().ToArray();

        foreach(Transform t in transforms)
        {
            plots.Add(t.gameObject.GetComponent<GardenPlot>());
        }
    }

    public void PushData()
    {
        List<int> itemIds = new();
        List<int> plotIndeces = new();
        List<int> growthStages = new();
        List<float> growthProgress = new();

        for(int i = 0; i < plots.Count; i++)
        {
            Seeds seeds = plots[i].Seeds;
            if(seeds != null)
            {
                itemIds.Add(plots[i].Seeds.id);
                plotIndeces.Add(i);
                growthStages.Add(plots[i].CurrentStage);
                float gProgress = plots[i].growthProgress + WorldClock.WorldTimeSince(plots[i].lastGrowthAttempt);
                growthProgress.Add(gProgress);
            }
            
        }

        persist.ItemIds = itemIds;
        persist.PlotIndeces = plotIndeces;
        persist.GrowthStages = growthStages;
        persist.GrowthProgress = growthProgress;
        persist.UnloadTime = Time.time;
    }

    private void PullData()
    {
        int n = persist.PlotIndeces.Count;
        for(int i = 0; i < n; i++)
        {
            int index = persist.PlotIndeces[i];
            Seeds s = (Seeds)Instantiate(ItemDictionary.items[persist.ItemIds[i]]);
            plots[index].Seeds = s;
            plots[index].CurrentStage = persist.GrowthStages[i];
            plots[index].growthProgress = persist.GrowthProgress[i];
            plots[index].unloadTime = persist.UnloadTime;
            plots[index].RestartGrowth();
        }
    }
}
