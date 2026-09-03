using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct JobRequestContainer
{
    public string Description { get; set; }
    public int ItemQty { get; set; }
    public int Reward { get; set; }
    public Item RequestedItem { get; set; }
    public float Deadline { get; set; }

    public JobRequestContainer(JobRequest jobRequest)
    {
        Description = jobRequest.description;
        ItemQty = jobRequest.ChosenItemQty;
        Reward = jobRequest.ChosenReward;
        RequestedItem = jobRequest.requestedItem;
        Deadline = -1;
    }
}
