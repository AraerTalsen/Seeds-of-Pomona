using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Scriptable Objects/PUp")]
public class PUp : Item, IRuntimeLauncher
{
    [SerializeField] protected EffectParameters parameters;
    public enum EffectLabel { stat, transform, instantiate, status }
    
    [SerializeField] private EffectLabel effectLabel;
    [SerializeReference] public Affecter affecter = new TransformAffecter();

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
    //private string CleanName => name[..name.IndexOf("(")];

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

    public Task<IRuntimeEvent> LaunchEffect(EffectContext context) => affecter.CreateRuntimeEvent(context);
}
