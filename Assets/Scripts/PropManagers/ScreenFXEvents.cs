using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFXEvents : MonoBehaviour
{
    public void TransitionScene()
    {
        SceneManager.LoadScene("TheBase", LoadSceneMode.Single);
    }
}
