using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/JobBoardData")]
public class JobBoardData : ScriptableObject
{
    [SerializeField]
    private List<JobRequest> jobListings;
    [SerializeField]
    private List<InventoryEntry> requestedItems;
    [SerializeField]
    private float lastPostTime;
    [SerializeField]
    private bool isPersisting = false;

    public List<JobRequest> JobListings {get => jobListings; set => jobListings = value;}
    public List<InventoryEntry> RequestedItems {get => requestedItems; set => requestedItems = value;}
    public float LastPostTime {get => lastPostTime; set => lastPostTime = value;}
    public bool IsPersisting {get => isPersisting; set => isPersisting = value;}
}
