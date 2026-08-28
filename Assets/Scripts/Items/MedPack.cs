using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Scriptable Objects/Tools/Med Pack")]
public class MedPack : Tool
{
    [SerializeField][TextArea] private string invalidUseMessage;
    public override void ToolAction(EffectContext context)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        bool isWilderness = sceneName.CompareTo("Wilderness") == 0;
        bool isArena = sceneName.CompareTo("TestArena") == 0;
        EntityStats stats = context.Owner.Body.GetComponent<EntityStats>();
        bool isMaxHealth = stats.MaxHealth == stats.CurrentHealth;
        if(isWilderness || isArena && !isMaxHealth)
        {
            stats.CurrentHealth++;   
        }
        else if(!isWilderness || !isArena)
        {
            TextWindowManager.Instance.SetMessage(invalidUseMessage, context.Owner.Worldbox.GetComponent<Move_Player>());
        }
    }
}
