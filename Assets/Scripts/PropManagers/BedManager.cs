using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedManager : Interactable
{
    private void Sleep(GameObject interactor)
    {
        interactor.transform.position = transform.position;
        interactor.GetComponent<PlayerSleepManager>().Sleep();
    }

    public override void StartInteractiveProcess(GameObject interactor)
    {
        Sleep(interactor);
    }
}
