using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuadEdge
{
    private QuarterEdge[] qtrEdges = {new(), new(), new(), new()};

    public QuarterEdge FirstQuarterEdge => qtrEdges[0];
    public string name;

    private GameObject line, vertex;

    public QuadEdge(Vertex pointA, Vertex pointB)
    {
        name = pointA.name + pointB.name;
        qtrEdges[0].data = pointA.point;
        qtrEdges[0].dest = pointB.point;
        qtrEdges[2].data = pointB.point;
        qtrEdges[2].dest = pointA.point;

        Vector3[] perpPoints = CalculatePerpendicular(pointA.point, pointB.point);
        qtrEdges[1].data = perpPoints[1];
        qtrEdges[1].dest = perpPoints[0];
        qtrEdges[3].data = perpPoints[0];
        qtrEdges[3].dest = perpPoints[1];

        qtrEdges[0].name = pointA.name + pointB.name;
        qtrEdges[1].name = "LeftRight";
        qtrEdges[2].name = pointB.name + pointA.name;
        qtrEdges[3].name = "RightLeft";

        qtrEdges[0].next = qtrEdges[0];
        qtrEdges[2].next = qtrEdges[2];

        qtrEdges[1].next = qtrEdges[3];
        qtrEdges[3].next = qtrEdges[1];

        InitializeQuarterEdges();
        BuildLine();
    }

    public void Stringify()
    {
        foreach(QuarterEdge q in qtrEdges) q.Stringify();
    }

    private void InitializeQuarterEdges()
    {
        LoadRotPointers();
        LoadRemainingPointers();
    }

    private void LoadRotPointers()
    {
        for(int i = 0; i < qtrEdges.Length; i++)
        {
            int index = i == qtrEdges.Length - 1 ? 0 : i + 1;
            qtrEdges[i].rot = qtrEdges[index];
        }
    }

    private void LoadRemainingPointers()
    {
        foreach(QuarterEdge q in qtrEdges)
        {
            q.sym = q.rot.rot;
            q.tor = q.sym.rot;
            q.prev = q.rot.next.rot;
            q.lnext = q.tor.next.rot;
            q.quad = this;
        }
    } 

    private Vector3[] CalculatePerpendicular(Vector3 p1, Vector3 p2)
    {
        Vector2 mP = new Vector2((p1.x + p2.x) / 2, (p1.z + p2.z) / 2);

        Vector2 dV = new Vector2(p2.x - p1.x, p2.z - p1.z);

        dV = new Vector2(-dV.y, dV.x);

        float halfLength = Mathf.Sqrt(dV.x * dV.x + dV.y * dV.y) / 2;
        dV = dV.normalized * halfLength;

        Vector3 point1 = new Vector3(mP.x + dV.x, 0, mP.y + dV.y);
        Vector3 point2 = new Vector3(mP.x - dV.x, 0, mP.y - dV.y);

        return new Vector3[] { point1, point2 };
    }

    public void ToggleLine()
    {
        line.SetActive(!line.activeSelf);
    }

    private void BuildLine()
    {
        Vector3 startP = FirstQuarterEdge.data;
        Vector3 endP = FirstQuarterEdge.dest;
        Vector3[] poses = {startP, endP};

        GenerateVertexObj();
        vertex.transform.position = poses[0];

        line = new()
        {
            name = "Quad"
        };
        line.transform.position = (poses[0] - poses[1]).normalized * (Vector3.Distance(poses[0], poses[1]) / 2);
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.SetPositions(poses);
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.startColor = Color.white;
        lr.endColor = Color.white;
    }

    private void GenerateVertexObj()
    {
        vertex = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/OriginalPoints", typeof(GameObject)));
        vertex.transform.localScale *= 12.5f;
    }   
}
