using System.Collections;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Move_Player))]
public class PlayerDeathManager : EntityDeathManager
{
    [SerializeField]
    private Animator screenFX;
    [SerializeField]
    private CinemachineVirtualCamera vcam;

    [SerializeField] private PlayerInventory inventory;
    private Move_Player mp;

    private void Start()
    {
        mp = GetComponent<Move_Player>();
    }

    protected override void KillEntity()
    {
        if(stats.CurrentHealth <= 0)
        {
            isDying = true;
            StartShakeyCam();
            screenFX.SetTrigger("hasDied");
            mp.TogglePauseMovement(true);
            inventory.TriggerDeath();
            inventory.ClearInventory();
            //inventory.PushDataTemp();
        }
    }

    private void StartShakeyCam()
    {
        CinemachineBasicMultiChannelPerlin noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 1.0f;
        StartCoroutine(ResetShake(noise));
    }

    private IEnumerator ResetShake(CinemachineBasicMultiChannelPerlin noise)
    {
        yield return new WaitForSeconds(0.2f);
        noise.m_AmplitudeGain = 0;
    }
}
