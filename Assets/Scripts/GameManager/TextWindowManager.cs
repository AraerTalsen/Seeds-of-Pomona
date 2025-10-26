using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextWindowManager : MonoBehaviour
{
    [SerializeField]
    private GameObject dialogueWindow;
    [SerializeField]
    private TMP_Text text;
    private string[] fragments;
    private int fragmentIndex = 0;
    private GameObject player;

    public static TextWindowManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void SetMessage(string message, GameObject player)
    {
        fragmentIndex = 0;
        dialogueWindow.SetActive(true);
        this.player = player;
        player.GetComponent<Move_Player>().TogglePauseMovement();
        BeginDialogueChain(message);
    }

    private void BeginDialogueChain(string message)
    {
        fragments = message.Split("\\n");
        LoadNextFragment();
    }

    public void LoadNextFragment()
    {
        bool isFinished = fragmentIndex >= fragments.Length;
        text.text = !isFinished ? fragments[fragmentIndex++] : "";

        if (isFinished)
        {
            dialogueWindow.SetActive(false);
            player.GetComponent<Move_Player>().TogglePauseMovement();
            player = null;
        }
    }
}
