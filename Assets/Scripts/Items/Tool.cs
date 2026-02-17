using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : Item
{
    [SerializeField] private bool isActive = true;
    [SerializeField] private int durability;
    public bool IsActive => isActive;
    public int Durability => durability;
    public int ExpirationDay { get; private set; } = -1;
    public bool IsExpired => ExpirationDay > -1 && ExpirationDay <= TimerObserver.Instance.CurrentDay;
    public override string CurrentToolTip => altToolTip.CompareTo("") != 0 && UseAltToolTip ? 
            string.Format(altToolTip, name[..name.IndexOf("(")], ExpirationDay - TimerObserver.Instance.CurrentDay) :
            string.Format(toolTip, isActive ? "Active" : "Passive", name[..name.IndexOf("(")], durability);

    public void SetExpirationDay(int day)
    {
        ExpirationDay = day;
        UseAltToolTip = true;
    }
    public abstract void UseAbility(GameObject user); 
}
