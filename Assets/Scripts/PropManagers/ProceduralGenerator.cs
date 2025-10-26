using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour
{
    [SerializeField]
    private LayerMask checkForCollisions;
    public int width, height;
    public GameObject[] props;
    public int[] baseSpawnQty;
    public int[] spawnQtyMods;


    private void Start()
    {
        GenerateProps();
    }

    private void GenerateProps()
    {
        for(int i = 0; i < props.Length; i++)
        {
            int mod = Random.Range(-spawnQtyMods[i], spawnQtyMods[i] + 1);
            int qty = Mathf.Max(1, baseSpawnQty[i] + mod);

            for (int j = 0; j < qty; j++)
            {
                int x = Random.Range(-width / 2 + 1, width / 2);
                int y = Random.Range(-height / 2 + 1, height / 2);
                Vector2 spawnPoint = new(x - 0.5f, y - 0.5f);
                if (Physics.OverlapSphere(spawnPoint, 0.9f, checkForCollisions).Length == 0)
                {
                    Instantiate(props[i], spawnPoint, Quaternion.identity);
                }
                else
                {
                    FindClearSpace(props[i], spawnPoint);
                }

            }
        }
    }

    private void FindClearSpace(GameObject g, Vector2 origin)
    {
        float x = origin.x;
        float y = origin.y;
        Vector2[] mods = {Vector2.right -Vector2.right}; 
        Vector2 newSpawn = origin;
        int attempts = 100;

        while (IsInBounds(x, y) && attempts > 0)
        {
            (int index, int qty) dirWithLeastObjs = (-1, 10000);

            for (int i = 0; i < mods.Length; i++)
            {
                Vector3 temp = newSpawn + mods[i];
                Collider[] objs = Physics.OverlapSphere(temp, 0.45f, checkForCollisions);
                if (objs.Length == 0)
                {
                    Instantiate(g, temp, Quaternion.identity);
                    return;
                }
                else
                {
                    dirWithLeastObjs = objs.Length < dirWithLeastObjs.qty ? (i, objs.Length) : dirWithLeastObjs;
                }
            }

            newSpawn += mods[dirWithLeastObjs.index];
            attempts--;
            x = newSpawn.x;
            y = newSpawn.y;
        }
    }

    private bool IsInBounds(float x, float y)
    {
        bool inHorizontalBounds = x >= -width / 2 + 1 && x <= width / 2;
        bool inVerticalBounds = y >= -height / 2 + 1 && y <= height / 2;

        return inHorizontalBounds && inVerticalBounds;
    }
}
