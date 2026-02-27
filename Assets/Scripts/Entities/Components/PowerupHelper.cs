using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupHelper : MonoBehaviour
{
    private List<bool> coolDowns = new();
    private List<SelectSlot> pUpSlots = new();
    private List<Tool> powerups = new();
    private List<float> aggTime = new();

    public PowerupContext Context { get; set; }

    private void Update()
    {
        UpdateCoolDownProgress();
    }
    
    public void TryAddCoolDown(SelectSlot slot, Tool tool)
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

    public void TryUseAbility(Tool tool, int slotIndex, GameObject player)
    {
        if(!coolDowns[slotIndex] && tool.CoolDown > 0)
        {
            StartCoroutine(UseAbility(tool, slotIndex, player));
        }
    }
    
    private IEnumerator UseAbility(Tool tool, int slotIndex, GameObject player)
    {
        ToggleCoolDown(slotIndex); 
        //Implement runner component
        //tool.UseAbility(Context);
        yield return new WaitForSeconds(tool.CoolDown);
        ToggleCoolDown(slotIndex);
        pUpSlots[slotIndex].CoolDownProgress.value = 0;
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
            }
        }
    }
}
