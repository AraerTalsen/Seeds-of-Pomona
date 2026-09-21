using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SeparatorManager : PersistentObject<SeparatorData>
{
    [SerializeField] private SeparatorData persist;
    [SerializeField] private int processTime;
    [SerializeField] private Transform invContainerInput, invContainerOutput;
    private bool isProcessing = false;
    private float unloadTime = 0, timePassed = 0, carryOverProgress = 0, startTime;
    private Vector2Int filledOutputSlots = Vector2Int.zero;
    private BoundedDDI input, output;
    private Separator menu;
    
    public bool IsProcessing => isProcessing;
    public int ProcessTime => processTime;
    public float TimePassed { get => timePassed; set => timePassed = value; }

    protected override void PullData()
    {
        input = new(invContainerInput);
        output = new(invContainerOutput, true);
        output.Listener.SubscribeToChanges(SlotWasEmptied, InventoryListener.SlotTouchMode.Set);

        if(!Persist.IsPersisting)
        {
            Persist.Input = input.Entries;
            Persist.Output = output.Entries;
        }
        else
        {
            unloadTime = Persist.UnloadTime;
            timePassed = Persist.CurrentProgress;
            input.LoadFromStorage(Persist.Input);
            output.LoadFromStorage(Persist.Output);
            IsAnyOutputFull();
        }

        
        if (input.Read(0) != null && input.Read(0).Quantity > 0)
        {
            CalculateProgress();
        }
    }

    protected override void PushData()
    {
        Persist.IsPersisting = true;
        Persist.UnloadTime = Time.time;
        Persist.CurrentProgress = timePassed;
    }

    public void Initialize(Separator separator)
    {
        Persist = RetrieveData(persist);
        PullData();
        menu = separator;
    }
    
    public void Tick() => ProcessItems();

    public void OnDisable()
    {
        PushData();
    }

    public void ToggleProcessCheck()
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
        startTime = Time.time;
    }

    private void StopProcess()
    {
        isProcessing = false;
        menu.ResetProgress();
    }

    private void ProcessItems()
    {
        if (WorldClock.WorldTimeSince(startTime) >= (processTime - carryOverProgress) && input.Read(0).Quantity > 0)
        {
            carryOverProgress = 0;
            PullFromInputSlot(1);
            startTime = Time.time;
        }
        else if(input.Read(0).Quantity == 0)
        {
            isProcessing = false;
        }
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
            output.PushItems(outputs[0], randQty, out _);
            output.PushItems(outputs[1], 1, out _);

            if(IsAnyOutputFull())
            {
                StopProcess();
                unfulfilled = numItems - i - 1;
                break;
            }
        }
        return unfulfilled;
    }

    private void CalculateProgress()
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

    private void SlotWasEmptied()
    {
        IsAnyOutputFull();
    }

    private bool IsAnyOutputFull()
    {
        filledOutputSlots.x = GetFilledValue(output.Read(0));
        filledOutputSlots.y = GetFilledValue(output.Read(1));

        return filledOutputSlots.x > 0 
            || filledOutputSlots.y > 0; 
    }

    private int GetFilledValue(InventoryEntry slot) =>
        (slot != null && !slot.IsEmpty && 
        slot.Quantity >= slot.Item.maxStackSize) ? 1 : 0;
}
