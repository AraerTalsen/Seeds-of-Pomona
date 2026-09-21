using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;


[System.Serializable]
public class JobBoardDisplay
{
    [SerializeField] private Transform jobBoard;
    private Transform jobPostingGroup;
    private Image itemImg;
    private Button complete;
    private GameObject leftArrow, rightArrow, emptyBoard;
    public int currentListingIndex = 0;
    public UnityAction CompleteJobRequest;

    public List<JobRequestContainer> JobListings { get; set; }
    public TMP_Text Description { get; private set; }
    public TMP_Text ItemQty { get; private set; }
    public TMP_Text QtyInInv { get; private set; }
    public TMP_Text Reward { get; private set; }
    public bool JobListingsActiveSelf { get; private set; }
    public JobBoardProperties JobBoardProperties { get; set; }

    public void Initialize()
    {
        if(jobPostingGroup == null)
        {
            jobPostingGroup = jobBoard.GetChild(0);
            Description = jobPostingGroup.GetChild(1).GetComponent<TMP_Text>();
            ItemQty = jobPostingGroup.GetChild(2).GetChild(0).GetComponent<TMP_Text>();
            QtyInInv = jobPostingGroup.GetChild(3).GetComponent<TMP_Text>();
            Reward = jobPostingGroup.GetChild(4).GetComponent<TMP_Text>();
            itemImg = jobPostingGroup.GetChild(2).GetComponent<Image>();
            complete = jobPostingGroup.GetChild(5).GetComponent<Button>();

            leftArrow = jobBoard.GetChild(1).gameObject;
            rightArrow = jobBoard.GetChild(2).gameObject;

            emptyBoard = jobBoard.GetChild(3).gameObject;

            complete.onClick.AddListener(CompleteJobRequest);

            JobListingsActiveSelf = false;   
        }
    }

    public void UpdateArrowButtons()
    {
        bool isLargeEnough = JobListings.Count >= 2;        
        leftArrow.SetActive(isLargeEnough);
        rightArrow.SetActive(isLargeEnough);
    }

    public void UpdateCompleteButton()
    {
        complete.interactable = JobBoardProperties.FulfilledRequests.Contains(currentListingIndex);
    }

    public void NextJob()
    {
        currentListingIndex = (currentListingIndex + 1) % JobListings.Count;
        LoadJob();
    }

    public void PrevJob()
    {
        currentListingIndex = currentListingIndex - 1 > -1 ? currentListingIndex - 1 : JobListings.Count - 1;
        LoadJob();
    }

    //Display current job player has cycled to on the job board
    public void LoadJob()
    {
        JobRequestContainer job = JobListings[currentListingIndex];

        Description.text = job.Description;
        ItemQty.text = job.ItemQty.ToString();
        QtyInInv.text = "In Inventory: " + JobBoardProperties.CurrentPlayerInv[job.RequestedItem.id].ToString();
        Reward.text = "Reward: $" + job.Reward.ToString();
        itemImg.sprite = job.RequestedItem.sprite;
        UpdateCompleteButton();
    }

    //Activates job listings, as opposed to the empty job board message
    public void ActivateJobBoard()
    {
        if (JobListings.Count > 0)
        {
            ToggleJobListings(true);
            UpdateArrowButtons();
        }
    }

    public void ClearBoard()
    {
        currentListingIndex = 0;
        jobPostingGroup.gameObject.SetActive(false);
        emptyBoard.SetActive(true);
        JobListingsActiveSelf = false;
    }

    //Toggles between displaying the job listings and the empty job board message
    public void ToggleJobListings(bool isJobAvailable)
    {
        jobPostingGroup.gameObject.SetActive(isJobAvailable);
        emptyBoard.SetActive(!isJobAvailable);

        JobListingsActiveSelf = isJobAvailable;
    }
}
