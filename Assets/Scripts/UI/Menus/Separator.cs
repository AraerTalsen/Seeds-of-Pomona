using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//Machine that takes an input item and creates up to two types of output items from it.
public class Separator : BasicMenu
{
    //[SerializeField] private SeparatorData persist;
    //[SerializeField] private Transform invContainerInput;
    //[SerializeField] private Transform invContainerStdOutput;
    //public int processTime;
    public int machineId;
    //private bool isProcessing = false;
    [SerializeField] private Image stdProgressBar;
    //private float unloadTime = 0, timePassed = 0, carryOverProgress = 0;
    //private BoundedDDI input;
    //private BoundedDDI stdOutput;
    //private Vector2Int filledOutputSlots = Vector2Int.zero;
    [SerializeField] private SeparatorManager manager;

    protected void Start()
    {
        /*Persist = RetrieveData(persist);
        PullData();*/
        manager.Initialize(this);
    }

    private void FixedUpdate()
    {
        manager.ToggleProcessCheck();

        if (manager.IsProcessing)
        {
            manager.Tick();
            DisplayProgress();
        }
    }

    /*private void ToggleProcessCheck()
    {
        if (filledOutputSlots.magnitude == 0 && input.Read(0) != null && !input.Read(0).IsEmpty && !isProcessing)
        {
            StartProcess();
        }
        else if ((input.Read(0) == null || input.Read(0).IsEmpty) && isProcessing)
        {
            StopProcess();
        }
    }

    private void StartProcess()
    {
        isProcessing = true;
        StartCoroutine(nameof(ProcessItems));
    }

    private void StopProcess()
    {
        isProcessing = false;
        ResetProgress();
        StopCoroutine(nameof(ProcessItems));
    }

    private IEnumerator ProcessItems()
    {
        while (input.Read(0).Quantity > 0)
        {
            yield return new WaitForSeconds(processTime - carryOverProgress);
            carryOverProgress = 0;
            PullFromInputSlot(1);
        }
        isProcessing = false;
    }

    private void PullFromInputSlot(int numItems)
    {
        int unfulfilledGens = GenerateItems(numItems);
        input.PullItems(input.Read(0).Item.id, numItems - unfulfilledGens, out int unfulfilled);
    }

    private int GenerateItems(int numItems)
    {
        int unfulfilled = 0;
        int[] outputs = input.Read(0).Item.outputItems;

        for(int i = 0; i < numItems; i++)
        {
            int randQty = Mathf.Clamp(Random.Range(numItems, 3 * numItems + 1), 0, ItemDictionary.items[outputs[0]].maxStackSize);
            stdOutput.PushItems(outputs[0], randQty, out _);
            stdOutput.PushItems(outputs[1], 1, out _);

            if(IsAnyOutputFull())
            {
                StopProcess();
                unfulfilled = numItems - i - 1;
                break;
            }
        }
        return unfulfilled;
    }*/

    private void DisplayProgress()
    {
        manager.TimePassed += Time.deltaTime;
        float ratio = manager.TimePassed / manager.ProcessTime % 1;
        stdProgressBar.fillAmount = ratio;
    }

    public void ResetProgress()
    {
        manager.TimePassed = 0;
        stdProgressBar.fillAmount = 0;
    }

    /*private void CalculateProgress()
    {
        timePassed += WorldClock.WorldTimeSince(unloadTime);
        int numLoopsFinished = Mathf.Min(input.Read(0).Quantity, (int)timePassed / processTime);
        PullFromInputSlot(numLoopsFinished);
        if (input.Read(0).Quantity <= 0 || IsAnyOutputFull())
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

    protected override void PullData()
    {
        input = new(invContainerInput);
        stdOutput = new(invContainerStdOutput, true);
        stdOutput.Listener.SubscribeToChanges(SlotWasEmptied, InventoryListener.SlotTouchMode.Set);

        if(!Persist.IsPersisting)
        {
            Persist.Input = input.Entries;
            Persist.Output = stdOutput.Entries;
        }
        else
        {
            unloadTime = Persist.UnloadTime;
            timePassed = Persist.CurrentProgress;
            input.LoadFromStorage(Persist.Input);
            stdOutput.LoadFromStorage(Persist.Output);
            IsAnyOutputFull();
        }

        
        if (input.Read(0) != null && input.Read(0).Quantity > 0)
        {
            CalculateProgress();
        }
    }

    private void SlotWasEmptied()
    {
        IsAnyOutputFull();
    }

    private bool IsAnyOutputFull()
    {
        filledOutputSlots.x = GetFilledValue(stdOutput.Read(0));
        filledOutputSlots.y = GetFilledValue(stdOutput.Read(1));

        return filledOutputSlots.x > 0 
            || filledOutputSlots.y > 0; 
    }

    private int GetFilledValue(InventoryEntry slot) =>
        (slot != null && !slot.IsEmpty && 
        slot.Quantity >= slot.Item.maxStackSize) ? 1 : 0;

    /*protected override void PushData()
    {
        Persist.IsPersisting = true;
        Persist.UnloadTime = Time.time;
        Persist.CurrentProgress = timePassed;
    }*/

    public void OnDisable() => manager.OnDisable();
}