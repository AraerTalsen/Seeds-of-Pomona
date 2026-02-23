using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//Machine that takes an input item and creates up to two types of output items from it.
public class Separator : PersistentObject<SeparatorData>
{
    [SerializeField] private SeparatorData persist;
    [SerializeField] private Transform invContainerInput;
    [SerializeField] private Transform invContainerStdOutput;
    [SerializeField] private Transform invContainerSpOutput;
    public int processTime;
    public int machineId;
    private bool isProcessing = false;
    [SerializeField] private Image stdProgressBar;
    [SerializeField] private Image spProgressBar;
    private float unloadTime = 0, timePassed = 0, carryOverProgress = 0;
    private BoundedDDI input;
    private BoundedDDI stdOutput;
    private BoundedDDI spOutput;
    private Vector3Int filledOutputSlots = Vector3Int.zero;

    protected void Start()
    {
        Persist = RetrieveData(persist);
        PullData();
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
        GenerateItems(numItems);
        input.PullItems(input.Read(0).Item.id, numItems, out int unfulfilled);
    }

    private void GenerateItems(int numItems)
    {
        int[] outputs = input.Read(0).Item.outputItems;
        int randQty = Mathf.Clamp(Random.Range(numItems, 3 * numItems + 1), 0, ItemDictionary.items[outputs[0]].maxStackSize);
        stdOutput.PushItems(outputs[0], randQty);
        stdOutput.PushItems(outputs[1], numItems);

        if(IsAnyOutputFull())
        {
            StopProcess();
        }
    }

    private void DisplayProgress()
    {
        timePassed += Time.deltaTime;
        float ratio = timePassed / processTime % 1;
        stdProgressBar.fillAmount = ratio;
        spProgressBar.fillAmount = ratio;
    }

    private void ResetProgress()
    {
        timePassed = 0;
        stdProgressBar.fillAmount = 0;
        spProgressBar.fillAmount = 0;
    }

    private void CalculateProgress()
    {
        timePassed += WorldClock.WorldTimeSince(unloadTime);
        int numLoopsFinished = Mathf.Min(input.Read(0).Quantity, (int)timePassed / processTime);
        PullFromInputSlot(numLoopsFinished);
        if (input.Read(0).Quantity <= 0)
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
        spOutput = new(invContainerSpOutput, true);
        stdOutput.Listener.SubscribeToChanges(SlotWasEmptied, InventoryListener.SlotTouchMode.Set);
        spOutput.Listener.SubscribeToChanges(SlotWasEmptied, InventoryListener.SlotTouchMode.Set);
        stdOutput.Listener.SubscribeToTouchedSlots(SlotWasEmptied, 0);
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
        }

        
        if (input.Read(0) != null && input.Read(0).Quantity > 0)
        {
            CalculateProgress();
        }
    }

    private void SlotWasEmptied()
    {
        stdOutput.Listener.PrintAllDetails();
        IsAnyOutputFull();
        print($"Inventory entry: {stdOutput.Read(0).GetHashCode()}, Slot #0 is {stdOutput.Read(0).Quantity} of {stdOutput.Read(0).Item}");
        print($"Vector: {filledOutputSlots}");
    }

    private bool IsAnyOutputFull()
    {
        filledOutputSlots.x = GetFilledValue(stdOutput.Read(0));
        filledOutputSlots.y = GetFilledValue(stdOutput.Read(1));
        filledOutputSlots.z = GetFilledValue(spOutput.Read(0));

        return filledOutputSlots.x > 0 
            || filledOutputSlots.y > 0 
            || filledOutputSlots.z > 0;
    }

    private int GetFilledValue(InventoryEntry slot) =>
        (slot != null && !slot.IsEmpty && 
        slot.Quantity >= slot.Item.maxStackSize) ? 1 : 0;

    protected override void PushData()
    {
        Persist.IsPersisting = true;
        Persist.UnloadTime = Time.time;
        Persist.CurrentProgress = timePassed;
    }

    public void PushDataTemp()
    {
        PushData();
    }
}