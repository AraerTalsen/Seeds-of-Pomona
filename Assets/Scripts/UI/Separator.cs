using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//Machine that takes an input item and creates up to two types of output items from it.
public class Separator : PersistentObject<SeparatorData>
{
    [SerializeField] private SeparatorData persist;
    [SerializeField] private Transform invContainerInput;
    [SerializeField] private Transform invContainerOutput;
    public int processTime;
    public int machineId;
    public int inputSize, outputSize;
    private bool isProcessing = false;
    [SerializeField] private Image progressBar;
    private float unloadTime = 0, timePassed = 0, carryOverProgress = 0;
    private BoundedDDI input;
    private BoundedDDI output;

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
        if (input.Read(0) != null && !input.Read(0).IsEmpty && !isProcessing)
        {
            StartProcess();
        }
        else if ((input.Read(0) == null || input.Read(0).IsEmpty) && isProcessing)
        {
            isProcessing = false;
            ResetProgress();
            StopCoroutine(nameof(ProcessItems));
        }
    }

    private void StartProcess()
    {
        isProcessing = true;
        StartCoroutine(nameof(ProcessItems));
    }

    private IEnumerator ProcessItems()
    {
        while (input.Read(0).Quantity > 0)
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
        input.PullItems(input.Read(0).Item.id, numItems, out int unfulfilled);
    }

    private void GenerateItems(int numItems)
    {
        int[] outputs = input.Read(0).Item.outputItems;
        int randQty = Mathf.Clamp(Random.Range(numItems, 3 * numItems + 1), 0, ItemDictionary.items[outputs[0]].maxStackSize);
        output.PushItems(outputs[0], randQty);
        output.PushItems(outputs[1], numItems);
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
        output = new(invContainerOutput);
        if(!Persist.IsPersisting)
        {
            Persist.Input = input.Entries.ToList();
            Persist.Output = output.Entries.ToList();
        }
        else
        {
            unloadTime = Persist.UnloadTime;
            timePassed = Persist.CurrentProgress;
            input.LoadFromStorage(Persist.Input);
            output.LoadFromStorage(Persist.Output);
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

    public void PushDataTemp()
    {
        PushData();
    }
}
