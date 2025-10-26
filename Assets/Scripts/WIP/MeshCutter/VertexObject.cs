using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Data object for storing neighboring verteces
public class VertexObject
{
    private Vector3 _position;
    private List<Vector3> _neighbors;

    public Vector3 Position
    {
        get
        {
            return _position;
        }
        set
        {
            _position = value;
        }
    }

    public List<Vector3> Neighbors
    {
        get
        {
            return _neighbors;
        }
        set
        {
            _neighbors = value;
        }
    }

    public VertexObject(Vector3 position, List<Vector3> neighbors = null)
    {
        _position = position;
        _neighbors = neighbors;
    }

    public void AddNeighbor(Vector3 neighbor)
    {
        if(_neighbors != null)
            _neighbors.Add(neighbor);
        else
        {
            _neighbors = new List<Vector3>{neighbor};
        }
    }

    public void AddNeighbors(Vector3[] neighbors)
    {
        for(int i = 0; i < neighbors.Length; i++)
            AddNeighbor(neighbors[i]);
    }

    public void RemoveNeighbor(Vector3 neighbor, int index = -1)
    {
        if(index > -1)
            _neighbors.RemoveAt(index);
        else    
            _neighbors.Remove(neighbor);
    }

    public void ClearNeighbors()
    {
        _neighbors.Clear();
    }

    public void PrintNeighbors()
    {
        foreach(Vector3 v in  _neighbors)
            Debug.Log(v);
    }
}
