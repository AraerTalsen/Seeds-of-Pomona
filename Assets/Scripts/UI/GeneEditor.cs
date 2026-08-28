using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneEditor : PersistentObject<GeneEditorData>
{
    [SerializeField] private GeneEditorData persist;
    [SerializeField] private Transform invContainerInput;
    [SerializeField] private Transform invContainerFailedOutput;
    [SerializeField] private Transform invContainerSucceededOutput;
    [SerializeField] private int successOdds;
    public int processTime;
    public int machineId;
    private bool isProcessing = false;
    [SerializeField] private Image stdProgressBar;
    private float unloadTime = 0, timePassed = 0, carryOverProgress = 0;
    private BoundedDDI input;
    private BoundedDDI failOutput, successOutput;
    //private BoundedDDI spOutput;
    private Vector2Int filledOutputSlots = Vector2Int.zero;

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
                successOutput.PushItems(outputs[1], 1, out _, true);
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

    private void DisplayProgress()
    {
        timePassed += Time.deltaTime;
        float ratio = timePassed / processTime % 1;
        stdProgressBar.fillAmount = ratio;
    }

    private void ResetProgress()
    {
        timePassed = 0;
        stdProgressBar.fillAmount = 0;
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

    private void SlotWasEmptied()
    {
        //failOutput.Listener.PrintAllDetails();
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

    protected override void PushData()
    {
        Persist.IsPersisting = true;
        Persist.UnloadTime = Time.time;
        Persist.CurrentProgress = timePassed;
    }

    private void OnDisable()
    {
        PushData();
    }
}
