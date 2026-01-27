using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerObserver : MonoBehaviour
{
    public static TimerObserver Instance {get; private set;}
    private List<ITimer> subscriptions = new();

    public int CurrentDay {get; private set;}
    
    private void Awake()
    {
        TrySetInstance();
    }

    private void TrySetInstance()
    {
        if (Instance != null && Instance != this)
        {
            print("Duplicate Timer Observer");
            Destroy(this);
        }
        else
        {
            print("New Timer Observer");
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void Broadcast()
    {
        CurrentDay++;
        foreach(ITimer timer in subscriptions)
        {
            timer.IncrementTime();
        }
    }

    public void Subscribe(ITimer timer)
    {
        subscriptions.Add(timer);
    }

    public void Unsubscribe(ITimer timer)
    {
        subscriptions.Remove(timer);
    }

    public bool CheckStatus(ITimer timer)
    {
        return subscriptions.Find(t => t == timer) != null;
    }
}
