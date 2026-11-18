using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

[System.Serializable]
public class JobBoardManager : PersistentObject<JobBoardData>, ITimer
{
    [SerializeField] private JobBoardData persist;
    [SerializeField] private int intervalInDays, maxJobCapacity;
    [SerializeField] private JobBoardDisplay jobBoardDisplay;
    private List<JobRequestContainer> jobListings;
    private List<InventoryEntry> requestedItems;
    private List<(int requestedItmIndex, int itemId)?> fulfilledItems;

    public int IntervalInDays {get => intervalInDays;}
    public JobBoardDisplay JobBoardDisplay => jobBoardDisplay;
    private PlayerInventory inv;
    private BusinessOperationsManager busOp;
    private JobBoardProperties jobBoardProperties;
    private ShopManager shopManager;

     public void Initialize(ShopManager shop, BusinessOperationsManager operations)
    {
        Persist = RetrieveData(persist);
        jobBoardProperties = new();
        shopManager = shop;
        busOp = operations;
        PullData();

        InitializeDisplayManager();
        TimerObserver.Instance.Broadcast();
    }

    protected override void PullData()
    {
        TimerObserver.Instance.Subscribe(this);

        jobListings = new();
        requestedItems = new();
        if (!Persist.IsPersisting)
        {
            Persist.IsPersisting = true;
            Persist.JobListings = jobListings;
            Persist.RequestedItems = requestedItems;
        }
        else
        {
            jobListings = Persist.JobListings;
            requestedItems = Persist.RequestedItems;
        }
        shopManager.InitializeDisplay(Persist.IsGeneEditorEnabled);
    }

    public void InitializeDisplayManager()
    {
        jobBoardDisplay.CompleteJobRequest = CompleteJobRequest;
        busOp.CheckPlayerInventory = CheckPlayerInventory;
        jobBoardDisplay.JobBoardProperties = jobBoardProperties;
        jobBoardDisplay.JobListings = jobListings;
    }

    public void IncrementTime()
    {
        Debug.Log("Day: " + TimerObserver.Instance.CurrentDay);
        if (TimerObserver.Instance.CurrentDay % IntervalInDays == 0 && jobListings.Count < maxJobCapacity)
        {
            CreateNewJobRequest();
        }
    }

    //What's a more streamlined way of dynamically removing all items from a list that meet a certain condition? Can it be done in a single pass?
    public void ClearOldJobs()
    {
        List<JobRequestContainer> removeJobs = new();
        for (int i = 0; i < jobListings.Count; i++)
        {
            if (jobListings[i].Deadline <= TimerObserver.Instance.CurrentDay)
            {
                removeJobs.Add(jobListings[i]);
            }
        }

        for (int i = 0; i < removeJobs.Count; i++)
        {
            RemoveJobRequest(removeJobs[i]);
        }
    }

    private void CreateNewJobRequest()
    {
        int rand = Random.Range(0, JobDictionary.jobs.Count);
        JobRequestContainer newJob = new(JobDictionary.jobs[rand])
        {
            Deadline = TimerObserver.Instance.CurrentDay + JobDictionary.jobs[rand].timeline
        };
        AddJobRequest(newJob);
    }

    private void AddJobRequest(JobRequestContainer job)
    {
        requestedItems.Add(new InventoryEntry(job.ItemQty, job.RequestedItem));
        jobListings.Add(job);

        if (busOp.isOpen)
        {
            CheckPlayerInventory();
            if (!jobBoardDisplay.JobListingsActiveSelf)
            {
                jobBoardDisplay.ToggleJobListings(true);
            }
            jobBoardDisplay.LoadJob();
            jobBoardDisplay.UpdateArrowButtons();
        }
    }

    //Check if player has any of the necessary items to complete the current jobs
    public void CheckPlayerInventory()
    {
        inv = inv == null ? busOp.Inv : inv;
        jobBoardProperties.FulfilledRequests = new();
        jobBoardProperties.CurrentPlayerInv.Clear();

        if (inv != null)
        {
            for (int i = 0; i < requestedItems.Count; i++)
            {
                int id = requestedItems[i].Item.id;
                if(inv.Find(id) != null)
                {
                    TryFulfillRequest(id, i);
                }
                else
                {
                    if(!jobBoardProperties.CurrentPlayerInv.ContainsKey(id))
                    {
                        jobBoardProperties.CurrentPlayerInv[id] = 0;
                    }
                }
            }
        }
    }

    private void TryFulfillRequest(int id, int index)
    {
        int itemSum = inv.Sum(id);
        if(!jobBoardProperties.CurrentPlayerInv.ContainsKey(id))
        {
            jobBoardProperties.CurrentPlayerInv[id] = itemSum;
        }

        if(requestedItems[index].Quantity <= itemSum)
        {
            jobBoardProperties.FulfilledRequests.Add(index);
        } 
        
    }

    public void CompleteJobRequest()
    {
        inv.Wallet.IncrementBalance(jobListings[jobBoardDisplay.currentListingIndex].Reward);
        busOp.UpdatePlayerWallet();
        TakeItemsFromPlayer();
        RemoveJobRequest();
        shopManager.UpdateDisplays();
    }

    private void TakeItemsFromPlayer()
    {
        int itemId = requestedItems[jobBoardDisplay.currentListingIndex].Item.id;
        int qty = requestedItems[jobBoardDisplay.currentListingIndex].Quantity;
        inv.PullItems(itemId, qty, out int unfulfilled);
    }

    private void RemoveJobRequest()
    {
        jobListings.RemoveAt(jobBoardDisplay.currentListingIndex);
        requestedItems.RemoveAt(jobBoardDisplay.currentListingIndex);
        
        if (jobListings.Count >= 1 && busOp.isOpen)
        {
            CheckPlayerInventory();
            jobBoardDisplay.UpdateArrowButtons();
            jobBoardDisplay.PrevJob();
        }
        else if(jobListings.Count == 0)
        {
            jobBoardDisplay.ClearBoard();
        }
    }

    private void RemoveJobRequest(JobRequestContainer job)
    {
        int index = jobListings.FindIndex(j => j.Equals(job));
        jobListings.RemoveAt(index);
        requestedItems.RemoveAt(index);

        if (jobListings.Count >= 1)
        {
            if (busOp.isOpen)
            {
                jobBoardDisplay.UpdateArrowButtons();

                if (jobListings.Count > 0)
                {
                    jobBoardDisplay.PrevJob();
                }
                else if (jobListings.Count == 0)
                {
                    jobBoardDisplay.ClearBoard();
                }
                CheckPlayerInventory();
            }
        }
    }

    protected override void PushData()
    {
        Persist.IsGeneEditorEnabled = shopManager.IsGeneEditorEnabled;
    }

    public void OnDisable()
    {
        PushData();
        TimerObserver.Instance.Unsubscribe(this);
    }
}
