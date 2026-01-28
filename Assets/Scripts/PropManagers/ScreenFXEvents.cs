using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFXEvents : MonoBehaviour
{
    [SerializeField]
    private PlayerSleepManager playerSleepManager;

    public void TransitionScene()
    {
        SceneManager.LoadScene("TheBase", LoadSceneMode.Single);
    }

    public void WakeUp()
    {
        playerSleepManager.WakeUp();
    }
}
