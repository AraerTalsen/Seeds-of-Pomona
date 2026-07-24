using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFXEvents : MonoBehaviour
{
    [SerializeField]
    private PlayerSleepManager playerSleepManager;

    public void TransitionScene()
    {
        string name = SceneManager.GetActiveScene().name;

        switch (name)
        {
            case "Wilderness":
                {
                    SceneManager.LoadScene("TheBase", LoadSceneMode.Single);
                    break;
                }
            case "TestArena":
                {
                    SceneManager.LoadScene("TestArena", LoadSceneMode.Single);
                    break;
                }
        }
        
    }

    public void WakeUp()
    {
        playerSleepManager.WakeUp();
    }
}
