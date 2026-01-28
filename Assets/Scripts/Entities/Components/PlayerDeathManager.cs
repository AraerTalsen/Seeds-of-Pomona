using System.Collections;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(Move_Player))]
public class PlayerDeathManager : MonoBehaviour
{
    [SerializeField]
    private Animator screenFX;
    [SerializeField]
    private CinemachineVirtualCamera vcam;

    private PlayerInventory inventory;
    private Move_Player mp;

    private void Start()
    {
        inventory = GetComponent<PlayerInventory>();
        mp = GetComponent<Move_Player>();
    }

    public void KillPlayer()
    {
        StartShakeyCam();
        screenFX.SetTrigger("hasDied");
        mp.TogglePauseMovement(true);
        inventory.TriggerDeath();
        inventory.ClearInventory();
        inventory.PushData();
    }

    private void StartShakeyCam()
    {
        CinemachineBasicMultiChannelPerlin noise;
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 1.0f;
        StartCoroutine(ResetShake(noise));
    }

    private IEnumerator ResetShake(CinemachineBasicMultiChannelPerlin noise)
    {
        yield return new WaitForSeconds(0.2f);
        noise.m_AmplitudeGain = 0;
    }
}
