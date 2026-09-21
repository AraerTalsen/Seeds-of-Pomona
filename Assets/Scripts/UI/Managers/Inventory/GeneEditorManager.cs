using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneEditorManager : PersistentObject<GeneEditorData>
{
    [SerializeField] private GeneEditorData persist;
    [SerializeField] private Transform invContainerInput, invContainerFailedOutput, invContainerSucceededOutput;
    [SerializeField] private int processTime, successOdds;
    private BoundedDDI input, failOutput, successOutput;
    private bool isProcessing = false;
    private float unloadTime = 0, timePassed = 0, carryOverProgress = 0, startTime;
    private Vector2Int filledOutputSlots = Vector2Int.zero;
    private GeneEditor menu;

    public bool IsProcessing => isProcessing;
    public int ProcessTime => processTime;
    public float TimePassed { get => timePassed; set => timePassed = value; }

    protected override void PullData()
    {
        input = new(invContainerInput);
        failOutput = new(invContainerFailedOutput, true);
        failOutput.Listener.SubscribeToChanges(SlotWasEmptied, InventoryListener.SlotTouchMode.Set);
        successOutput = new(invContainerSucceededOutput, true);
        successOutput.Listener.SubscribeToChanges(SlotWasEmptied, InventoryListener.SlotTouchMode.Set);

        if(!Persist.IsPersisting)
        {
            Persist.Input = input.Entries;
            Persist.FailOutput = failOutput.Entries;
            Persist.SuccessOutput = successOutput.Entries;
        }
        else
        {
            unloadTime = Persist.UnloadTime;
            timePassed = Persist.CurrentProgress;
            input.LoadFromStorage(Persist.Input);
            failOutput.LoadFromStorage(Persist.FailOutput);
            successOutput.LoadFromStorage(Persist.SuccessOutput);
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

    public void Initialize(GeneEditor geneEditor)
    {
        menu = geneEditor;
        Persist = RetrieveData(persist);
        PullData();
    }

    public void Tick() => ProcessItems();

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
        if(WorldClock.WorldTimeSince(startTime) >= (processTime - carryOverProgress) && input.Read(0).Quantity > 0)
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
            int rand = Random.Range(0, 100);
            if(rand > successOdds)//fail
            {
                failOutput.PushItems(12, 1, out _);
            }
            else//success
            {
                successOutput.PushItems(outputs[1], 1, out _);
            }

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
        filledOutputSlots.x = GetFilledValue(failOutput.Read(0));
        filledOutputSlots.y = GetFilledValue(successOutput.Read(0));

        return filledOutputSlots.x > 0 
            || filledOutputSlots.y > 0;
    }

    private int GetFilledValue(InventoryEntry slot) =>
        (slot != null && !slot.IsEmpty && 
        slot.Quantity >= slot.Item.maxStackSize) ? 1 : 0;

    public void OnDisable()
    {
        PushData();
    }
}
