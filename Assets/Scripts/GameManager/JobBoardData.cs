using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/JobBoardData")]
public class JobBoardData : PersistentDataBase
{
    [SerializeField]
    private List<JobRequestContainer> jobListings;
    [SerializeField]
    private List<InventoryEntry> requestedItems;
    [SerializeField]
    private float lastPostTime;
    [SerializeField]
    private bool isPersisting = false;

    public List<JobRequestContainer> JobListings {get => jobListings; set => jobListings = value;}
    public List<InventoryEntry> RequestedItems {get => requestedItems; set => requestedItems = value;}
    public bool IsPersisting {get => isPersisting; set => isPersisting = value;}
}
