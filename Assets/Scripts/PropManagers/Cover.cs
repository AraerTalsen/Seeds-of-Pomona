using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Move_Player mp = other.gameObject.GetComponent<Move_Player>();
            mp.ToggleHidden();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Move_Player mp = other.gameObject.GetComponent<Move_Player>();
            mp.ToggleHidden();
        }
    }
}
