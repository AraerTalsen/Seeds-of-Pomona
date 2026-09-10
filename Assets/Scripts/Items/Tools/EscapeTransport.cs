using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Scriptable Objects/Tools/Escape Transport")]
public class EscapeTransport : Tool
{
    [SerializeField][TextArea] private string invalidUseMessage;
    public override void ToolAction(EffectContext context)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        bool isWilderness = sceneName.CompareTo("Wilderness") == 0;
        bool isArena = sceneName.CompareTo("TestArena") == 0;

        if(isWilderness || isArena)
        {
            SceneManager.LoadScene("TheBase", LoadSceneMode.Single);
        }
        else if(!isWilderness || !isArena)
        {
            TextWindowManager.Instance.SetMessage(invalidUseMessage, context.Owner.Worldbox.GetComponent<Move_Player>());
        }
    }
}
