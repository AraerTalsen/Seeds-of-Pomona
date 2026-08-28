using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

//Manage JobRequest creation and organization 
public class JobBoardManager : PersistentObject<JobBoardData>, ITimer
{
    [SerializeField] private JobBoardData persist;
    private List<JobRequestContainer> jobListings;
    private List<InventoryEntry> requestedItems;
    private List<(int requestedItmIndex, int itemId)?> fulfilledItems;

    //private float lastPostTime = 0;
    [SerializeField]
    private int intervalInDays;
    public int IntervalInDays {get => intervalInDays;}
    private PInv inv;
    public int maxJobCapacity;
    private JobBoardDisplay jobBoardDisplay;
    private BusinessOperationsnManager busOp;
    private JobBoardProperties jobBoardProperties;
    private ShopManager shopManager;

    private void Start()
    {
        Persist = RetrieveData(persist);
        jobBoardProperties = new();
        shopManager = GetComponent<ShopManager>();
        busOp = GetComponent<BusinessOperationsnManager>();
        PullData();

        //Remove this line when the bed is added back in
        IncrementTime();
    }

    private void FixedUpdate()
    {
        ClearOldJobs();
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
            if(Persist.IsGeneEditorEnabled) shopManager.Initialize();
        }
    }

    public void InitializeDisplayManager()
    {
        jobBoardDisplay = busOp.JobBoardDisplay;
        jobBoardDisplay.CompleteJobRequest = CompleteJobRequest;
        busOp.CheckPlayerInventory = CheckPlayerInventory;
        jobBoardDisplay.JobBoardProperties = jobBoardProperties;
        jobBoardDisplay.JobListings = jobListings;
    }

    public void IncrementTime()
    {
        print("Day: " + TimerObserver.Instance.CurrentDay);
        if (TimerObserver.Instance.CurrentDay % IntervalInDays == 0 && jobListings.Count < maxJobCapacity)
        {
            CreateNewJobRequest();
        }
    }

    //What's a more streamlined way of dynamically removing all items from a list that meet a certain condition? Can it be done in a single pass?
    private void ClearOldJobs()
    {
        List<JobRequestContainer> removeJobs = new();
        for (int i = 0; i < jobListings.Count; i++)
        {
            if (jobListings[i].Deadline <= TimerObserver.Instance.CurrentDay/*WorldClock.WorldTimeInSeconds*/)
            {
                removeJobs.Add(jobListings[i]);
            }
        }

        for (int i = 0; i < removeJobs.Count; i++)
        {
            RemoveJobRequest(removeJobs[i]/*i*/);
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

        //Should no longer be possible
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
        //fulfilledItems = new();

        if (inv != null)
        {
            for (int i = 0; i < requestedItems.Count; i++)
            {
                int id = requestedItems[i].Item.id;//inv.GetInventory().Find(requestedItems[i].Item.id);
                //(int, int)? temp = inv.GetInventory().Find(id) == null ? null : (i, id);
                //fulfilledItems.Add(temp);
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
            //FindFulfilledRequests();
        }
    }

    //Find all jobs where the player has the correct quantity of the requested items
    /*private void FindFulfilledRequests()
    {
        if (fulfilledItems.Count > 0)
        {
            jobBoardProperties.FulfilledRequests = new();

            for (int i = 0; i < requestedItems.Count; i++)
            {
                if (fulfilledItems[i] != null)
                {
                    int itemId = fulfilledItems[i].Value.itemId;
                    int heldItemQty = inv.GetInventory().Sum(itemId);
                    if (requestedItems[i].Quantity <= heldItemQty)
                    {
                        jobBoardProperties.FulfilledRequests.Add(fulfilledItems[i].Value.itemId);
                    }
                }
            }
            
            jobBoardDisplay.UpdateCompleteButton();
        }
    }*/

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
        inv.wallet.IncrementBalance(jobListings[jobBoardDisplay.currentListingIndex].Reward);
        busOp.UpdatePlayerWallet();
        TakeItemsFromPlayer();
        RemoveJobRequest();
        shopManager.UpdateDisplays();
    }

    private void TakeItemsFromPlayer()
    {
        //Might be passing invalid item id
        int itemId = requestedItems[jobBoardDisplay.currentListingIndex].Item.id;
        int qty = requestedItems[jobBoardDisplay.currentListingIndex].Quantity;
        inv.PullItems(itemId, qty, out int unfulfilled);
    }

    //Comments for proposed replacement to RemoveJobRequest(JobRequest job)
    private void RemoveJobRequest(/*int listingIndex = jobBoardDisplay.currentListingIndex*/)
    {
        jobListings.RemoveAt(jobBoardDisplay.currentListingIndex/*listingIndex*/);
        requestedItems.RemoveAt(jobBoardDisplay.currentListingIndex/*listingIndex*/);
        //foreach(JobRequest j in jobListings) print($"Job, Request: {j.requestedItem}, {j.ChosenItemQty} Reward: {j.ChosenReward}");
        
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
        //foreach(JobRequest j in jobListings) print($"Job, Request: {j.requestedItem}, {j.ChosenItemQty} Reward: {j.ChosenReward}");

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

    private void OnDisable()
    {
        PushData();
        TimerObserver.Instance.Unsubscribe(this);
    }
}
