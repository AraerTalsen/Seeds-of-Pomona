using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventRunner : MonoBehaviour
{
    private static readonly List<IRuntimeEvent> runningEvents = new();
    public static EventRunner Instance { get; private set; }

    void Awake()
    {
        TrySetInstance();
    }

    private void TrySetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public static void Run(IRuntimeEvent e)
    {
        runningEvents.Add(e);
    }

    public static void Terminate(IRuntimeEvent e)
    {
        if(runningEvents.Contains(e)) runningEvents.Remove(e);
    }

    private void Update()
    {
        TickEffects();
    }

    private void TickEffects()
    {
        for (int i = runningEvents.Count - 1; i >= 0; i--)
        {
            IRuntimeEvent runtime = runningEvents[i];
            
            if(runtime != null && runtime.Owner != null)
            {
                runtime.Tick();

                if (runtime.IsFinished)
                    runningEvents.RemoveAt(i);
            }
            else
            {
                runningEvents.RemoveAt(i);
            }
        }
    }
}
