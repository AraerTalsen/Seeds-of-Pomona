using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : Item, IAbilityEffect
{
    [SerializeField] private bool isActive = true;
    [SerializeField] private int durability;
    [SerializeField] private int coolDown;
    public bool IsActive => isActive;
    public int Durability => durability;
    public int CoolDown => coolDown;
    public int ExpirationDay { get; private set; } = -1;
    public bool IsExpired => ExpirationDay > -1 && ExpirationDay <= TimerObserver.Instance.CurrentDay;
    private string defaultTip = "[{0}]\n{1} will last for {2} expeditions";
    private string altTip = "{0} has {1} expeditions remaining";

    public void SetExpirationDay(int day)
    {
        ExpirationDay = day;
        UseAltToolTip = true;
    }
    protected override string GetToolTip() => TipFormatter();

    private string TipFormatter()
    {
        if(UseAltToolTip)
        {
            return string.Format(altTip, name, ExpirationDay - TimerObserver.Instance.CurrentDay);
        }

        return string.Format(defaultTip, isActive ? "Active" : "Passive", name, Durability);
    }

    public abstract IEffectRuntime CreateEffectRuntime(EffectContext context);  
}
