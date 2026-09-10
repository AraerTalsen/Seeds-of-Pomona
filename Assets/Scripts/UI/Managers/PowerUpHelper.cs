using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PowerUpHelper : MonoBehaviour
{
    private List<bool> coolDowns = new();
    private List<SelectSlot> pUpSlots = new();
    private List<PowerUp> powerups = new();
    private List<float> aggTime = new();

    public Func<EffectContext> Context { get; set; }

    private void Update()
    {
        UpdateCoolDownProgress();
    }
    
    public void TryAddCoolDown(SelectSlot slot, PowerUp tool)
    {
        coolDowns.Add(false);
        pUpSlots.Add(slot);
        powerups.Add(tool);
        aggTime.Add(0);
    }

    public void TryRemoveCoolDown(int index)
    {
        coolDowns.RemoveAt(index);
        pUpSlots.RemoveAt(index);
        powerups.RemoveAt(index);
        aggTime.RemoveAt(index);
    }

    public void ToggleCoolDown(int index) => coolDowns[index] = !coolDowns[index];

    public async void TryUseAbility(PowerUp tool, int slotIndex)
    {
        if(!coolDowns[slotIndex] && tool.CoolDown > 0)
        {
            try
            {
                IRuntimeEvent runtimeEvent = await tool.LaunchEffect(Context());
                ToggleCoolDown(slotIndex); 
                EventRunner.Run(runtimeEvent);
            }
            catch(Exception e)
            {
                Debug.LogError($"Failed to receieve event: {e}");   
            }
        }
    }

    private void UpdateCoolDownProgress()
    {
        for(int i = 0; i < coolDowns.Count; i++)
        {
            if(coolDowns[i])
            {
                aggTime[i] += Time.deltaTime;
                float ratio = aggTime[i] / powerups[i].CoolDown % 1;
                pUpSlots[i].CoolDownProgress.value =  1.0f - ratio;

                if(aggTime[i] / powerups[i].CoolDown >= 1.0f)
                {
                    ToggleCoolDown(i);
                    pUpSlots[i].CoolDownProgress.value = 0;
                    aggTime[i] = 0;
                }
            }
        }
    }
}
