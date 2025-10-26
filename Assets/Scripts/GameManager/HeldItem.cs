using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeldItem 
{
    public int id;
    public int totalQty;
    public List<Vector2> coords;

    public HeldItem(int ID, int TotalQty, List<Vector2> Coords)
    {
        id = ID;
        totalQty = TotalQty;
        coords = Coords;
    }

    public void Deconstruct(out int ID, out int TotalQty, out List<Vector2> Coords)
    {
        ID = id;
        TotalQty = totalQty;
        Coords = coords;
    }
}
