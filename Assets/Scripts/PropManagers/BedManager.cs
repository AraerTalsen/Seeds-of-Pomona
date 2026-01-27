using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedManager : Interactable
{
    private void Sleep()
    {
        
    }

    public override void StartInteractiveProcess(GameObject interactor)
    {
        print("Sleeping");
        Sleep();
        TimerObserver.Instance.Broadcast();
    }
}
