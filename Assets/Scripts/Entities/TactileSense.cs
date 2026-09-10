using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TactileSense : MonoBehaviour
{
    public List<GameObject> ActiveCollisions { get; } = new();
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject g = collision.gameObject;
        if(!ActiveCollisions.Contains(g)) ActiveCollisions.Add(g);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject g = collision.gameObject;
        if(ActiveCollisions.Contains(g)) ActiveCollisions.Remove(g);
    }
}
