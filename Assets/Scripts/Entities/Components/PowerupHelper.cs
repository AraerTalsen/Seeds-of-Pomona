using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupHelper : MonoBehaviour
{
    private List<bool> coolDowns = new();
    private List<Tool> pUpSlots = new();

    private void Update()
    {
        UpdateCoolDownProgress();
    }
    
    public void TryAddCoolDown(Tool tool)
    {
        coolDowns.Add(false);
        pUpSlots.Add(tool);
    }

    public void TryRemoveCoolDown(Tool tool, int index)
    {
        coolDowns.RemoveAt(index);
        pUpSlots.RemoveAt(index);
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
        tool.UseAbility(player);
        yield return new WaitForSeconds(tool.CoolDown);
        ToggleCoolDown(slotIndex);
    }

    private void UpdateCoolDownProgress()
    {
        for(int i = 0; i < coolDowns.Count; i++)
        {
            if(coolDowns[i])
            {
                
            }
        }
    }
}
