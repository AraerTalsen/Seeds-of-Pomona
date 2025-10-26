using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//Machine that takes an input item and creates up to two types of output items from it.
public class Separator : Inventory
{
    public int processTime;
    public int machineId;
    private SeparatorData overridePersist;
    private bool isProcessing = false;
    private Image progressBar;
    private float unloadTime = 0, timePassed = 0, carryOverProgress = 0;

    protected override void Start()
    {
        InitializeInvStorage();
        persist = RetrievePersistentData();
        overridePersist = (SeparatorData)persist;
        InitializeSaveData(overridePersist);
    }

    private void FixedUpdate()
    {
        ToggleProcessCheck();

        if (isProcessing)
        {
            DisplayProgress();
        }
    }

    private void ToggleProcessCheck()
    {
        if (inventories[0][0] != null && !inventories[0][0].IsEmpty && !isProcessing)
        {
            StartProcess();
        }
        else if ((inventories[0][0] == null || inventories[0][0].IsEmpty) && isProcessing)
        {
            isProcessing = false;
            ResetProgress();
            StopCoroutine("ProcessItems");
        }
    }

    private void StartProcess()
    {
        isProcessing = true;
        if (progressBar == null)
        {
            progressBar = DisplayManager.ActiveMenu.transform.GetChild(1).GetChild(1).GetComponent<Image>();
        }
        StartCoroutine("ProcessItems");
    }

    private IEnumerator ProcessItems()
    {
        while (Peek(0, 0).Quantity > 0)
        {
            yield return new WaitForSeconds(processTime - carryOverProgress);
            carryOverProgress = 0;
            PullFromInputSlot(1);
        }
        //UpdateSlotQty(0, 0, 0, inventories[0][0].item);
        isProcessing = false;
    }

    //We should revise our code to update InventoryEntries rather han creating new ones 
    private void PullFromInputSlot(int numItems)
    {
        GenerateItems(numItems);
        UpdateSlotQty(-numItems, 0, 0, Peek(0, 0).item);
    }

    private void GenerateItems(int numItems)
    {
        int[] outputs = Peek(0, 0).item.outputItems;
        int randQty = Mathf.Clamp(Random.Range(numItems, 3 * numItems + 1), 0, ItemDictionary.items[outputs[0]].maxStackSize);
        AddItem(randQty, outputs[0], 1);
        AddItem(numItems, outputs[1], 1);
    }

    private void DisplayProgress()
    {
        timePassed += Time.deltaTime;
        float ratio = timePassed / processTime % 1;
        progressBar.fillAmount = ratio;
    }

    private void ResetProgress()
    {
        timePassed = 0;
        progressBar.fillAmount = 0;
    }

    private void CalculateProgress()
    {
        timePassed += WorldClock.WorldTimeSince(unloadTime);
        int numLoopsFinished = Mathf.Min(Peek(0, 0).Quantity, (int)timePassed / processTime);
        PullFromInputSlot(numLoopsFinished);
        if (Peek(0, 0).Quantity <= 0)
        {
            timePassed = 0;
        }
        else
        {
            timePassed %= processTime;
            carryOverProgress = timePassed;
            StartProcess();
        }
    }

    public void PushData()
    {

        overridePersist.IsPersisting = true;
        overridePersist.UnloadTime = Time.time;
        overridePersist.CurrentProgress = timePassed;
    }

    public override void PullData()
    {
        unloadTime = overridePersist.UnloadTime;
        timePassed = overridePersist.CurrentProgress;
        if (inventories[0][0] != null && inventories[0][0].Quantity > 0)
        {
            CalculateProgress();
        }
    }
}
