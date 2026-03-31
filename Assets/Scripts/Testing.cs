using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private void Start()
    {
        FindNearestOnMesh(Vector2.zero);
    }

    private void FindNearestOnMesh(Vector2 origin)
    {
        Vector2[] mods = { Vector2.up, Vector2.right, Vector2.down, Vector2.left }; 
        int maxDist = 3;
        int currentDist = 1;
        List<Vector2> prevChecks = new() { origin };
        List<Vector2> allChecks = new() { origin };
        List<Vector2> curChecks = new();

        while (currentDist < maxDist)
        {
            for (int i = 0; i < prevChecks.Count; i++)
            {
                for (int j = 0; j < mods.Length; j++)
                {
                    Vector2 newPoint = prevChecks[i] + mods[j];
                    if(!allChecks.Contains(newPoint))
                    {
                        curChecks.Add(newPoint);
                        allChecks.Add(newPoint);
                    }
                }
            }
            
            prevChecks = new(curChecks);
            curChecks.Clear();
            
            currentDist++;
        }
    }
}
