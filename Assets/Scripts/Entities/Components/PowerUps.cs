using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PowerUps : FlexDDI
{
    private int scrollIndex = 0;

    public PowerUps(GameObject player, int capacity, Transform invContainer, FlexInvDisplayManager.ISlotPrefill prefill) :
    base(capacity, invContainer, prefill)
    {
        this.player = player;
    }

    //lock and unlock slots
    //use powerups passively
    //display HUD slots

    private List<int> activePUpIndeces = new();
    private GameObject player;

    public override void SetSlot(InventoryEntry entry, int slotIndex)
    {
        InventoryEntry prevEntry = Read(slotIndex);
        base.SetSlot(entry, slotIndex);

        if(!entry.IsEmpty)
        {
            if(prevEntry.IsEmpty && !IsAtCapacity())
            {
                CreateNewEntry();
                ((FlexInvDisplayManager)DisplayManager).AddNewISlot();
            }
            
            if(SceneManager.GetActiveScene().name.CompareTo("Wilderness") == 0)
            {
                DisplayManager.ToggleLock(slotIndex);
                Tool t = (Tool)entry.Item;
                t.SetExpirationDay(TimerObserver.Instance.CurrentDay + t.Durability);
            }

            if(((Tool)entry.Item).IsActive)
            {
                activePUpIndeces.Add(slotIndex);
            }
        }
        else if(entry.IsEmpty)
        {
            Delete(slotIndex);

            if(!prevEntry.IsEmpty && ((Tool)prevEntry.Item).IsActive)
            {
                activePUpIndeces.Remove(slotIndex);
            }
        }
    }

    public override void Delete(int slotIndex)
    {
        if(slotIndex != Count - 1)
        {
            base.Delete(slotIndex);
            ((FlexInvDisplayManager)DisplayManager).RemoveSlotAt(slotIndex);
        }
        else
        {
            Read(slotIndex).Remove();
        }

        if(DisplayManager.IsSlotLocked(slotIndex))
        {
            DisplayManager.ToggleLock(slotIndex);
        }
    }

    public void RebuildSlots(List<InventoryEntry> storedData, List<bool> lockStates)
    {
        (storedData, lockStates) = CheckTimerExpirations(storedData, lockStates);
        LoadFromStorage(storedData);
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        for(int i = 0; i < ((FlexInvDisplayManager)DisplayManager).SlotCount; i++)
        {
            bool isWilderness = sceneName.CompareTo("Wilderness") == 0;
            bool isSlotInUse = !Read(i).IsEmpty;
            bool isSlotUnlocked = !lockStates[i];

            if(isWilderness && isSlotInUse && isSlotUnlocked)
            {
                DisplayManager.SetSlotLock(i, true);
                Tool t = (Tool)storedData[i].Item;
                t.SetExpirationDay(TimerObserver.Instance.CurrentDay + t.Durability);
            }
            else
            {
                DisplayManager.SetSlotLock(i, lockStates[i]);
            }
        }
    }

    private (List<InventoryEntry>, List<bool>) CheckTimerExpirations(List<InventoryEntry> entries, List<bool> lockStates)
    {
        for(int i = entries.Count - 1; i >= 0 ; i--)
        {
            if(!entries[i].IsEmpty && ((Tool)entries[i].Item).IsExpired)
            {
                if(i != entries.Count - 1)
                {
                    entries.RemoveAt(i);
                    lockStates.RemoveAt(i);

                    if(!entries[^1].IsEmpty)
                    {
                        entries.Add(InventoryEntry.Empty());
                        lockStates.Add(false);
                    }
                }
                else
                {
                    entries[i].Remove();
                    lockStates[i] = false;
                }
            }
        }

        return (entries, lockStates);
    }

    public override void LoadFromStorage(List<InventoryEntry> storedData)
    {
        base.LoadFromStorage(storedData);

        DisplayManager.UpdateItemDisplay(0);
        for(int i = 1; i < Count; i++)
        {
            ((FlexInvDisplayManager)DisplayManager).AddNewISlot();
        }
    }

    public bool IsSlotLocked(int slotIndex) => DisplayManager.IsSlotLocked(slotIndex);

    public void PowerupInterface()
    {
        ScrollPowerups();
        UsePowerup();
    }

    private void ScrollPowerups()
    {
        int scrollDelta = (int)Input.mouseScrollDelta.y;
        int pUpCount = activePUpIndeces.Count;

        if (pUpCount == 0)
        {
            scrollIndex = 0;
        }
        else if (scrollDelta != 0)
        {
            scrollIndex = (scrollIndex + scrollDelta) % pUpCount;

            if (scrollIndex < 0)
                scrollIndex += pUpCount;
        }
    }

    private void UsePowerup()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Tool t = (Tool)Read(activePUpIndeces[scrollIndex]).Item;
            t.UseAbility(player);
        }
    }
}
