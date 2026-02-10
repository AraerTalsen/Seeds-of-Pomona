using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Find way to include all of the referenced scripts into some modular package that can be interated through to call the PushData function
public class Doorway : Interactable
{
    [SerializeField]
    private bool autoOpenDoor = false;
    public string destination;
    public PInv playerInventory;
    public Separator separator;
    public GardenBed gardenBed;
    public JobBoardManager jobBoardManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (autoOpenDoor && other.CompareTag("Player"))
        {
            StartInteractiveProcess(other.gameObject);
        }
    }

    public override void StartInteractiveProcess(GameObject interactor)
    {
        playerInventory.PushDataTemp();
        if (gardenBed != null) gardenBed.PushData();
        if (separator != null) separator.PushDataTemp();
        SceneManager.LoadScene(destination, LoadSceneMode.Single);
    }
}
