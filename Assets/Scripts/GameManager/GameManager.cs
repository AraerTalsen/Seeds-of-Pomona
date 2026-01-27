using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //This script needs a DontDestroyOnLoad to persist the Instance on scene reloads
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        TrySetInstance();
    }

    private void TrySetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            InitializeGameSettings();
        }
    }

    private void InitializeGameSettings()
    {
        CursorManager.CurrentCursor = CursorManager.CursorType.DEFAULT;
    }
}
