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
        Sleep();
        TimerObserver.Instance.Broadcast();
    }
}
