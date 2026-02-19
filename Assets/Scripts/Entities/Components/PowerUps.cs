using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PowerUps : FlexDDI
{
    private int scrollIndex = 0;
    private string sceneName = SceneManager.GetActiveScene().name;
    private List<GameObject> activeHUDSlots = new();
    private GameObject HUDSlot;
    private Transform HUDContainer;

    public PowerUps(GameObject player, int capacity, Transform invContainer, FlexInvDisplayManager.ISlotPrefill prefill, GameObject HUDSlot, Transform HUDContainer) :
    base(capacity, invContainer, prefill)
    {
        this.player = player;
        this.HUDSlot = HUDSlot;
        this.HUDContainer = HUDContainer;
    }

    //lock and unlock slots
    //use powerups passively
    //display HUD slots

    private List<int> activePUpIndeces = new();
    private List<int> passivePUpIndeces = new();
    private GameObject player;

    public override void SetSlot(InventoryEntry entry, int slotIndex)
    {
        InventoryEntry prevEntry = Read(slotIndex);
        base.SetSlot(entry, slotIndex);

        if(!entry.IsEmpty)
        {
            Tool t = (Tool)entry.Item;

            if(prevEntry.IsEmpty && !IsAtCapacity())
            {
                CreateNewEntry();
                ((FlexInvDisplayManager)DisplayManager).AddNewISlot();
            }
            else if(!prevEntry.IsEmpty)
            {
                
            }
            
            if(sceneName.CompareTo("Wilderness") == 0)
            {
                DisplayManager.ToggleLock(slotIndex);
                t.SetExpirationDay(TimerObserver.Instance.CurrentDay + t.Durability);
                AddToolRefToSecondaryList(t, slotIndex);
            }

            
        }
        else if(entry.IsEmpty)
        {
            Delete(slotIndex);

            if(!prevEntry.IsEmpty)
            {
                Tool t = (Tool)prevEntry.Item;
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

        bool isWilderness = sceneName.CompareTo("Wilderness") == 0;
        for(int i = 0; i < storedData.Count; i++)
        {
            if(isWilderness && !storedData[i].IsEmpty)
            {
                AddToolRefToSecondaryList((Tool)storedData[i].Item, i);
            }
        }
        
        for(int i = 0; i < ((FlexInvDisplayManager)DisplayManager).SlotCount; i++)
        {
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
        UseActive();
        UsePassives();
    }

    private void UseActive()
    {
        //This will need to make a check for powerup cool downs
        //A powerup payload can be created to save instance data for such a check
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Tool t = (Tool)Read(activePUpIndeces[scrollIndex]).Item;
            t.UseAbility(player);
        }
    }

    private void UsePassives()
    {
        foreach(int i in passivePUpIndeces)
        {
            Tool t = (Tool)Read(i).Item;
            t.UseAbility(player);
        }
    }
    public override void ClearInventory()
    {
        base.ClearInventory();
        DisplayManager.SetSlotLock(0, false);
        activeHUDSlots.Clear();
    }

    private void AddToolRefToSecondaryList(Tool tool, int slotIndex)
    {
        if(tool.IsActive)
        {
            activePUpIndeces.Add(slotIndex);
            CreateNewHUDSlot(tool.sprite);
        }
        else
        {
            passivePUpIndeces.Add(slotIndex);
        }
    }

    private void RemoveToolRefFromSecondaryList(Tool tool, int slotIndex)
    {
        if(tool.IsActive)
        {
            int index = activePUpIndeces.FindIndex( i => i == slotIndex);
            activePUpIndeces.Remove(slotIndex);
            GameObject g = activeHUDSlots[index];
            activeHUDSlots.RemoveAt(index);
            Object.Destroy(g);
        }
        else
        {
            passivePUpIndeces.Remove(slotIndex);
        }
    }

    /*private void UpdateSecondaryList(Tool tool, int slotIndex)
    {
        if(tool.IsActive)
        {
            int index = activePUpIndeces.FindIndex( i => i == slotIndex);
            if(index == -1)
            {
                activePUpIndeces.Insert()
            }
            activeHUDSlots[index]
        }
    }*/

    private void CreateNewHUDSlot(Sprite sprite)
    {
        activeHUDSlots.Add(Object.Instantiate(HUDSlot, HUDContainer));
        activeHUDSlots[^1].transform.GetChild(0).GetComponent<Image>().sprite = sprite;
        
    }
}
