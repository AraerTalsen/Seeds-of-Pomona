using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuarterEdge
{
    public QuadEdge quad;
    public QuarterEdge rot, tor, sym, next, prev, lnext;
    public Vector3 data, dest;
    public string name;

    private GameObject line;

    public void Stringify()
    {
        Debug.Log($"Name: {name}\nStart: {data}\nEnd: {dest}\nRotation: {rot.name}\nOpposite: {sym.name}\nReverse: {tor.name}\nNext: {next.name}\nPrevious: {prev.name}\nNext Edge in Triangle: {lnext.name}");
    }

    public void ToggleLine()
    {
        if(line == null)
        {
            BuildLine();
            line.SetActive(false);
        }

        line.SetActive(!line.activeSelf);
        quad.ToggleLine();
    } 

    private void BuildLine()
    {
        Vector3 startP = data;
        Vector3 endP = dest;
        Vector3[] poses = {startP, endP};
        line = new GameObject();
        line.name = name;
        line.transform.position = (poses[0] - poses[1]).normalized * (Vector3.Distance(poses[0], poses[1]) / 2);
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.SetPositions(poses);
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.startColor = Color.white;
        lr.endColor = Color.red;
    }   
}
