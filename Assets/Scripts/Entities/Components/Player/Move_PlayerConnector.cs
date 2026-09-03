using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_PlayerConnector : MonoBehaviour
{
    [SerializeField] private Move_Player move_Player;
    public Move_Player Move_Player => move_Player;
}
