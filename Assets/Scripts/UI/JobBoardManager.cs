using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JobBoardDisplay))]

//Manage JobRequest creation and organization 
public class JobBoardManager : MonoBehaviour
{
    private List<JobRequest> jobListings;
    private List<InventoryEntry> requestedItems;
    private List<(int requestedIndex, int heldItmIndex)?> fulfilledItems;

    private float lastPostTime = 0;
    public int postDelayInSec;
    private PlayerInventory inv;
    public int maxJobCapacity;
    public JobBoardData persist;
    private JobBoardDisplay jobBoardDisplay;
    private JobBoardProperties jobBoardProperties;

    private void Start()
    {
        persist = RetrievePersistentData();
        jobBoardProperties = new();
        InitializeSaveData();
    }

    private void FixedUpdate()
    {
        ClearOldJobs();
        TimeForNewJob();
    }

    //Would Singleton patterns be better than our persistent data containers? They would eliminate these clunky data retrival checks in favor of
    //a quick instance comparison check, though, the container provides more accessibility and data persistence across game reloads
    protected JobBoardData RetrievePersistentData()
    {
        JobBoardData temp = (JobBoardData)PersistentData.RetrieveDataContainer(persist.GetType().ToString());
        if (temp == null)
        {
            PersistentData.AddDataContainer(persist);
            return persist;
        }
        else
        {
            return temp;
        }
    }

    private void InitializeSaveData()
    {
        if (!persist.IsPersisting)
        {
            jobListings = new();
            requestedItems = new();

            persist.IsPersisting = true;
            persist.JobListings = jobListings;
            persist.RequestedItems = requestedItems;
            InitializeDisplayManager();
        }
        else
        {
            jobListings = persist.JobListings;
            requestedItems = persist.RequestedItems;
            InitializeDisplayManager();
            PullData();
        }
    }

    private void InitializeDisplayManager()
    {
        jobBoardDisplay = GetComponent<JobBoardDisplay>();
        jobBoardDisplay.CompleteJobRequest = CompleteJobRequest;
        jobBoardDisplay.CheckPlayerInventory = CheckPlayerInventory;
        jobBoardDisplay.JobBoardProperties = jobBoardProperties;
        jobBoardDisplay.JobListings = jobListings;
    }

    //Check if enough time has passed to create a new job
    private void TimeForNewJob()
    {
        int timePassed = WorldClock.WorldTimeSince(lastPostTime);
        if (jobListings.Count < maxJobCapacity && timePassed > 1 && timePassed % postDelayInSec == 0)
        {
            CreateNewJobRequest();
        }
    }

    //What's a more streamlined way of dynamically removing all items from a list that meet a certain condition? Can it be done in a single pass?
    private void ClearOldJobs()
    {
        List<JobRequest> removeJobs = new();
        for (int i = 0; i < jobListings.Count; i++)
        {
            if (jobListings[i].Deadline <= WorldClock.WorldTimeInSeconds)
            {
                removeJobs.Add(jobListings[i]);
            }
        }

        for (int i = 0; i < removeJobs.Count; i++)
        {
            RemoveJobRequest(removeJobs[i]);
        }
    }

    private void CreateNewJobRequest(float postTime = -1)
    {
        lastPostTime = postTime == -1 ? Time.time : postTime;
        int rand = Random.Range(0, JobDictionary.jobs.Count);
        JobRequest newJob = Instantiate(JobDictionary.jobs[rand]);
        newJob.postedTime = lastPostTime;
        AddJobRequest(newJob);
    }

    private void AddJobRequest(JobRequest job)
    {
        requestedItems.Add(new InventoryEntry(job.ChosenItemQty, job.requestedItem));
        jobListings.Add(job);

        if (jobBoardDisplay.isOpen)
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
        inv = inv == null ? jobBoardDisplay.inv : inv;
        fulfilledItems = new();

        if (inv != null)
        {
            for (int i = 0; i < requestedItems.Count; i++)
            {
                int index = inv.FindItem(requestedItems[i].item.id);
                (int, int)? temp = index == -1 ? null : (i, index);
                fulfilledItems.Add(temp);
            }
            FindFulfilledRequests();
        }
    }

    //Find all jobs where the player has the correct quantity of the requested items
    private void FindFulfilledRequests()
    {
        if (fulfilledItems.Count > 0)
        {
            jobBoardProperties.FulfilledRequests = new();

            for (int i = 0; i < requestedItems.Count; i++)
            {
                if (fulfilledItems[i] != null)
                {
                    int heldItemQty = inv.itemsHeld[fulfilledItems[i].Value.heldItmIndex].totalQty;
                    if (requestedItems[i].Quantity <= heldItemQty)
                    {
                        jobBoardProperties.FulfilledRequests.Add(fulfilledItems[i].Value.requestedIndex);
                    }
                }
            }
            jobBoardDisplay.UpdateCompleteButton();
        }
    }

    public void CompleteJobRequest()
    {
        inv.wallet.IncrementBalance(jobListings[jobBoardDisplay.currentListingIndex].ChosenReward);
        TakeItemsFromPlayer();
        RemoveJobRequest();
    }

    private void TakeItemsFromPlayer()
    {
        int heldItmIndex = fulfilledItems[jobBoardDisplay.currentListingIndex].Value.heldItmIndex;
        int qty = requestedItems[jobBoardDisplay.currentListingIndex].Quantity;
        inv.PullItems(heldItmIndex, qty);
    }

    private void RemoveJobRequest()
    {
        jobListings.RemoveAt(jobBoardDisplay.currentListingIndex);
        requestedItems.RemoveAt(jobBoardDisplay.currentListingIndex);

        if (jobListings.Count >= 1 && jobBoardDisplay.isOpen)
        {
            jobBoardDisplay.UpdateArrowButtons();
            jobBoardDisplay.PrevJob();
            CheckPlayerInventory();
        }
        else if(jobListings.Count == 0)
        {
            jobBoardDisplay.ClearBoard();
        }
    }

    private void RemoveJobRequest(JobRequest job)
    {
        int index = jobListings.FindIndex(j => j == job);
        jobListings.RemoveAt(index);
        requestedItems.RemoveAt(index);

        if (jobListings.Count >= 1)
        {
            if (jobBoardDisplay.isOpen)
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

    //Calculate how many jobs have been added and removed from the board while the scene was unloaded
    private void CalculateJobDynamism()
    {
        ClearOldJobs();
        float timeSinceLastPost = WorldClock.WorldTimeSince(lastPostTime);
        int numLoopsFinished = (int)Mathf.Clamp(timeSinceLastPost / postDelayInSec, 0, 3 - jobListings.Count);
        for (int i = 0; i < numLoopsFinished; i++)
        {
            CreateNewJobRequest(lastPostTime + postDelayInSec);
        }
    }

    public void PushData()
    {
        persist.LastPostTime = lastPostTime;
    }

    private void PullData()
    {
        lastPostTime = persist.LastPostTime;
        CalculateJobDynamism();
    }
}
