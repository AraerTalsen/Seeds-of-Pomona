using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFXEvents : MonoBehaviour
{
    public void TransitionScene()
    {
        SceneManager.LoadScene("TheBase", LoadSceneMode.Single);
    }
}
