using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PowerUpDisplay : FlexDDI
{
    private int scrollIndex = 0;
    private string sceneName = SceneManager.GetActiveScene().name;
    private List<GameObject> activeHUDSlots = new();
    private GameObject HUDSlot;
    private Transform HUDContainer;
    private PowerUpHelper powerupHelper;

    public PowerUpDisplay(int capacity, Transform invContainer, FlexInvDisplayManager.ISlotPrefill prefill, GameObject HUDSlot, Transform HUDContainer, PowerUpHelper helper) :
    base(capacity, invContainer, prefill)
    {
        this.HUDSlot = HUDSlot;
        this.HUDContainer = HUDContainer;
        powerupHelper = helper;
    }

    private List<int> activePUpIndeces = new();
    private List<int> passivePUpIndeces = new();

    public override void SetSlot(int qty, Item item, int slotIndex)
    {
        InventoryEntry prevEntry = Read(slotIndex).Clone();
        base.SetSlot(qty, item, slotIndex);

        if(item != null)
        {
            PowerUp t = (PowerUp)item;

            if(prevEntry.IsEmpty && !IsAtCapacity())
            {
                CreateNewEntry();
                if(Listener != null)
                {
                    Listener.InventorySizeDelta++;
                }
                ((FlexInvDisplayManager)DisplayManager).AddNewISlot();
            }
            
            bool isWilderness = sceneName.CompareTo("Wilderness") == 0;
            bool isArena = sceneName.CompareTo("TestArena") == 0;
            if(isWilderness || isArena)
            {
                DisplayManager.ToggleLock(slotIndex);
                t.SetExpirationDay(TimerObserver.Instance.CurrentDay + t.Durability);
                AddToolRefToSecondaryList(t, slotIndex);
                powerupHelper.TryAddCoolDown(activeHUDSlots[slotIndex].GetComponent<SelectSlot>(), t);
            }            
        }
        else
        {
            Delete(slotIndex);
        }
    }
    //How to add RemoveToolRefFromSecondaryList to here
    public override void Delete(int slotIndex)
    {
        InventoryEntry prevEntry = Read(slotIndex).Clone();

        if(slotIndex != Count - 1)
        {
            base.Delete(slotIndex);
            ((FlexInvDisplayManager)DisplayManager).RemoveSlotAt(slotIndex);

            if(!Read(Count - 1).IsEmpty)
            {
                CreateNewEntry();
                if(Listener != null)
                {
                    Listener.InventorySizeDelta++;
                }
                ((FlexInvDisplayManager)DisplayManager).AddNewISlot();
            }
        }
        else
        {
            Listener?.StartNewEvent();
            if(Listener != null)
            {
                Listener.TempItem = Read(slotIndex).Item;
            }
            
            Read(slotIndex).Remove();
            Listener?.TouchSlot(slotIndex, Listener.TempItem, InventoryListener.SlotTouchMode.Cleared);
            DisplayManager.UpdateItemDisplay(slotIndex);

            if(DisplayManager.IsSlotLocked(slotIndex))
            {
                DisplayManager.ToggleLock(slotIndex);
            }
        }

        bool isWilderness = sceneName.CompareTo("Wilderness") == 0;
        bool isArena = sceneName.CompareTo("TestArena") == 0;
        if(!prevEntry.IsEmpty && (isWilderness || isArena))
        {
            PowerUp t = (PowerUp)prevEntry.Item;
            RemoveToolRefFromSecondaryList(t, slotIndex);
            powerupHelper.TryRemoveCoolDown(slotIndex);
        }

        if(scrollIndex == slotIndex)
        {
            scrollIndex = Mathf.Max(slotIndex - 1, 0);
            UpdateSelectedHUDSlot();
        }
    }

    public void RebuildSlots(List<InventoryEntry> storedData, List<bool> lockStates)
    {
        (storedData, lockStates) = CheckTimerExpirations(storedData, lockStates);
        LoadFromStorage(storedData);
        
        bool isWilderness = sceneName.CompareTo("Wilderness") == 0;
        bool isArena = sceneName.CompareTo("TestArena") == 0;
        for(int i = 0; i < storedData.Count; i++)
        {
            if((isWilderness || isArena) && !storedData[i].IsEmpty)
            {
                PowerUp tool = (PowerUp)storedData[i].Item;
                if(tool.ExpirationDay == -1)
                {
                    tool.SetExpirationDay(TimerObserver.Instance.CurrentDay + tool.Durability);
                }
                AddToolRefToSecondaryList(tool, i);
                powerupHelper.TryAddCoolDown(activeHUDSlots[i].GetComponent<SelectSlot>(), tool);
                if(!lockStates[i])
                {
                    lockStates[i] = true;
                }
            }
        }
        
        for(int i = 0; i < lockStates.Count; i++)
        {
           DisplayManager.SetSlotLock(i, lockStates[i]);
        }
    }

    private (List<InventoryEntry>, List<bool>) CheckTimerExpirations(List<InventoryEntry> entries, List<bool> lockStates)
    {
        for(int i = entries.Count - 1; i >= 0 ; i--)
        {
            if(!entries[i].IsEmpty && ((PowerUp)entries[i].Item).IsExpired)
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
        TryPowerupControls();
        UsePowerup();
    }

    private void TryPowerupControls()
    {
        ScrollPowerups();
        KeySelect();
    }

    private void ScrollPowerups()
    {
        int scrollDelta = 0 - (int)Input.mouseScrollDelta.y;
        int pUpCount = activePUpIndeces.Count;
        int tempIndex = scrollIndex;

        if (pUpCount == 0)
        {
            scrollIndex = 0;
        }
        else if (scrollDelta != 0 && Input.GetKey(KeyCode.LeftShift))
        {
            scrollIndex = (scrollIndex + scrollDelta) % pUpCount;

            if (scrollIndex < 0)
                scrollIndex += pUpCount;
        }

        if(scrollDelta != 0 && Input.GetKey(KeyCode.LeftShift) && pUpCount > 0)
        {
            UpdateSelectedHUDSlot(tempIndex);
        }
    }

    private void KeySelect()
    {
        if(Input.anyKeyDown && Input.GetKey(KeyCode.LeftShift))
        {
            int tempIndex = scrollIndex;
            for(int num = 1; num <= Entries.Count; num++)
            {
                if(Input.GetKeyDown(num.ToString()))
                {
                    scrollIndex = num - 1;
                    UpdateSelectedHUDSlot(tempIndex);
                    break;
                }
            }
        }
    }

    private void UpdateSelectedHUDSlot()
    {
        ToggleHUDSlot(scrollIndex);
    }

    private void UpdateSelectedHUDSlot(int prevIndex)
    {
        ToggleHUDSlot(prevIndex);
        ToggleHUDSlot(scrollIndex);
    }

    private void ToggleHUDSlot(int index)
    {
        activeHUDSlots[index].GetComponent<SelectSlot>().ToggleHighlight();
    }

    private void UsePowerup()
    {
        UseActive();
        UsePassives();
    }

    private void UseActive()
    {
        bool isWilderness = sceneName.CompareTo("Wilderness") == 0;
        bool isArena = sceneName.CompareTo("TestArena") == 0;
        if(Input.GetKeyDown(KeyCode.Q) && Count > 1 && (isWilderness || isArena))
        {
            int slotIndex = activePUpIndeces[scrollIndex];
            powerupHelper.TryUseAbility((PowerUp)Read(slotIndex).Item, slotIndex);
        }
    }

    private void UsePassives()
    {
        foreach(int i in passivePUpIndeces)
        {
            powerupHelper.TryUseAbility((PowerUp)Read(i).Item, i);
        }
    }

    
    public override void ClearInventory()
    {
        base.ClearInventory();
        DisplayManager.SetSlotLock(0, false);
        activeHUDSlots.Clear();
    }

    private void AddToolRefToSecondaryList(PowerUp tool, int slotIndex)
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

    private void RemoveToolRefFromSecondaryList(PowerUp prevTool, int slotIndex)
    {
        if(prevTool.IsActive)
        {
            int index = activePUpIndeces.FindIndex( i => i == slotIndex);
            activePUpIndeces.Remove(slotIndex);
            GameObject g = activeHUDSlots[index];
            activeHUDSlots.RemoveAt(index);
            Object.Destroy(g);
            for(int i = index; i < activePUpIndeces.Count; i++)
            {
                activePUpIndeces[i]--;
            }
        }
        else
        {
            int index = passivePUpIndeces.FindIndex( i => i == slotIndex);
            passivePUpIndeces.Remove(slotIndex);
            for(int i = 0; i < passivePUpIndeces.Count; i++)
            {
                passivePUpIndeces[i]--;
            }
        }
    }

    private void CreateNewHUDSlot(Sprite sprite)
    {
        activeHUDSlots.Add(Object.Instantiate(HUDSlot, HUDContainer));
        UpdateHUDSlotSprite(activeHUDSlots.Count - 1, sprite);
        if(activeHUDSlots.Count == 1)
        {
            ToggleHUDSlot(0);
        }
    }

    private void UpdateHUDSlotSprite(int powerupIndex, Sprite sprite)
    {
        activeHUDSlots[powerupIndex].GetComponent<SelectSlot>().SetSprite(sprite);
    }
}
