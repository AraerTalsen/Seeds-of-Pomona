using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;


public class JobBoardDisplay : BasicMenu
{
    public List<JobRequest> JobListings { get; set; }
    private Transform jobPostingGroup;
    public TMP_Text Description { get; private set; }
    public TMP_Text ItemQty { get; private set; }
    public TMP_Text Reward { get; private set; }
    public bool JobListingsActiveSelf { get; private set; }
    public JobBoardProperties JobBoardProperties { get; set; }
    private Image itemImg;
    private Button complete;
    private GameObject leftArrow, rightArrow, emptyBoard;
    public int currentListingIndex = 0;
    public PlayerInventory inv;
    public bool isOpen = false;

    public UnityAction CompleteJobRequest;
    public UnityAction CheckPlayerInventory;

    private void Start()
    {
        if (jobPostingGroup == null)
        {
            InitializeVariables();
        }
    }

    private void InitializeVariables()
    {
        jobPostingGroup = ActiveMenu.transform.GetChild(0);
        Description = jobPostingGroup.GetChild(1).GetComponent<TMP_Text>();
        ItemQty = jobPostingGroup.GetChild(2).GetChild(0).GetComponent<TMP_Text>();
        Reward = jobPostingGroup.GetChild(3).GetComponent<TMP_Text>();
        itemImg = jobPostingGroup.GetChild(2).GetComponent<Image>();
        complete = jobPostingGroup.GetChild(4).GetComponent<Button>();

        leftArrow = ActiveMenu.transform.GetChild(1).gameObject;
        rightArrow = ActiveMenu.transform.GetChild(2).gameObject;

        emptyBoard = ActiveMenu.transform.GetChild(3).gameObject;

        complete.onClick.AddListener(CompleteJobRequest);
        leftArrow.GetComponent<Button>().onClick.AddListener(PrevJob);
        rightArrow.GetComponent<Button>().onClick.AddListener(NextJob);

        JobListingsActiveSelf = false;
    }

    public override void ToggleMenu(GameObject menu2 = null, Move_Player mp = null)
    {
        base.ToggleMenu(menu2);

        isOpen = !isOpen;

        if (inv == null)
        {
            inv = mp.inventory;
        }
        CheckPlayerInventory();
        if (JobListings.Count > 0)
        {
            ActivateJobBoard();
            LoadJob();
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
        JobRequest job = JobListings[currentListingIndex];

        Description.text = job.description;
        ItemQty.text = job.ChosenItemQty.ToString();
        Reward.text = "Reward: $" + job.ChosenReward.ToString();
        itemImg.sprite = job.requestedItem.sprite;
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
