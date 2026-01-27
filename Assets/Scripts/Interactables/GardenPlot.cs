using UnityEngine;

//Carry over remaining time in CalculateGrowthStage to be the new growthProgress and continue growing from there
public class GardenPlot : Interactable, ITimer
{
    private Seeds seeds;
    private SpriteRenderer plant;
    private bool isGrowing = false, isFinished = false;
    public float lastGrowthAttempt = 0, growthProgress = 0, unloadTime = 0;
    private int growthRate, growthOdds;
    private int currentStage = 0;
    private Item output;
    private PlantInspection plantInspection;

    public Seeds Seeds { get => seeds; set => seeds = value; }
    public int CurrentStage { get => currentStage; set => currentStage = value; }

    [SerializeField]
    private int intervalInDays;
    public int IntervalInDays {get => intervalInDays;}

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        plant = child.GetComponent<SpriteRenderer>();
        plantInspection = child.GetComponent<PlantInspection>();
    }

    public void IncrementTime()
    {
        if (seeds != null && currentStage < seeds.growthStages.Length - 1)
        {
            TimeForNewAttempt();
        }
    }

    //Check if enough time passed for a new plant growth attempt
    private void TimeForNewAttempt()
    {
        if (TimerObserver.Instance.CurrentDay % IntervalInDays == 0/*WorldClock.WorldTimeSince(lastGrowthAttempt) + growthProgress >= growthRate*/)
        {
            //growthProgress = 0;
            AttemptNewGrowth();
        }
    }

    //Determine if plant will grow to next stage this growth period
    private void AttemptNewGrowth()
    {
        //lastGrowthAttempt = Time.time;
        if (growthOdds >= Random.Range(0, 100))
        {
            currentStage++;
            plantInspection.PlantGrowthStage = currentStage + 1;
            plant.sprite = seeds.growthStages[currentStage];

            CheckIfHarvestable();
        }
    }

    private bool CheckIfHarvestable()
    {
        if (currentStage >= seeds.growthStages.Length - 1)
        {
            isFinished = true;
            ToggleInteractability();
            return true;
        }

        return false;
    }

    public override void StartInteractiveProcess(GameObject interactor)
    {
        if (!isGrowing)
        {
            Inventory inv = interactor.GetComponent<Move_Player>().inventory;
            int index = inv.FindItem(0);
            if (index > -1)
            {
                seeds = (Seeds)inv.PullItems(index, 1).item;
                ToggleInteractability();
                StartGrowth();
            }
        }
        else if (isFinished)
        {
            Harvest(interactor);
        }
    }

    private void StartGrowth()
    {
        isGrowing = true;
        //lastGrowthAttempt = Time.time;
        InitializeValues();
    }

    private void InitializeValues()
    {
        plant.sprite = seeds.growthStages[currentStage];
        growthRate = seeds.growthRate;
        growthOdds = seeds.growthOdds;
        output = Instantiate(ItemDictionary.items[seeds.outputItems[0]]);
        plantInspection.PlantName = output.name.Split("(")[0];
        SetPlantInspectability();
    }

    private void SetPlantInspectability()
    {
        plantInspection.PlantGrowthStage = currentStage + 1;
        plantInspection.IsInspectable = !plantInspection.IsInspectable;
    }

    private void Harvest(GameObject interactor)
    {
        interactor.GetComponent<Move_Player>().inventory.AddItem((1, output), 0);
        plant.sprite = null;
        output = null;
        seeds = null;
        isGrowing = false;
        isFinished = false;
        currentStage = 0;
        SetPlantInspectability();
    }

    public void RestartGrowth()
    {
        ToggleInteractability();
        InitializeValues();
        
        if (!CheckIfHarvestable())
        {
            //CalculateGrowthStage();
            CheckIfHarvestable();
        }
        plant.sprite = seeds.growthStages[currentStage];
        isGrowing = true;
        //lastGrowthAttempt = Time.time;
    }

    //Simulate the number of successful growth attempts
    //1. Compare random number to the minimum odds to beat in order to grow x many stages
    //3. Clamp the growth potential by the number of growth cycles passed and the remaining growth stages of the plant
    /*private void CalculateGrowthStage()
    {
        float timeSinceUnload = WorldClock.WorldTimeSince(unloadTime);
        growthProgress += timeSinceUnload;

        int numCyclessFinished = (int)growthProgress / growthRate;

        float rand = Random.Range(0.0f, 100.0f) / 100;
        int n = (int)Mathf.Log(rand, growthOdds / 100.0f);
        int growthPotential = Mathf.Min(seeds.growthStages.Length - 1 - currentStage, numCyclessFinished);
        n = Mathf.Min(n, growthPotential);

        currentStage += n;
        growthProgress %= growthRate;
    }*/
}
