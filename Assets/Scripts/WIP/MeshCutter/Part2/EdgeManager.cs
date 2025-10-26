using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EdgeManager
{
    public static void Splice(QuarterEdge edge1, QuarterEdge edge2)
    {
        SwapNexts(edge1.next.rot, edge2.next.rot);
        SwapNexts(edge1, edge2);
    }

    private static void SwapNexts(QuarterEdge edge1, QuarterEdge edge2)
    {
        QuarterEdge edge1next = edge1.next;
        edge1.next = edge2.next;
        edge2.next = edge1next;
    }

    public static QuarterEdge MakeTriangle(Vertex pointA, Vertex pointB, Vertex pointC)
    {
        QuadEdge quadAB = new(pointA, pointB);
        QuadEdge quadBC = new(pointB, pointC);
        QuadEdge quadCA = new(pointC, pointA);

        QuarterEdge ab = quadAB.FirstQuarterEdge;
        QuarterEdge bc = quadBC.FirstQuarterEdge;
        QuarterEdge ca = quadCA.FirstQuarterEdge;

        Splice(ab.sym, bc);
        Splice(bc.sym, ca);
        Splice(ca.sym, ab);

        return ab;
    }

    public static QuarterEdge ConnectEdges(QuarterEdge edge1, QuarterEdge edge2)
    {
        QuadEdge newQuadEdge = new(new Vertex(edge1.dest, edge1.name[1].ToString()), new Vertex(edge2.data, edge2.name[0].ToString()));
        QuarterEdge newEdge = newQuadEdge.FirstQuarterEdge;

        Splice(newEdge, edge1.lnext);
        Splice(newEdge.sym, edge2);
        newEdge.Stringify();

        return newEdge;
    }

    public static void SeverEdges(QuarterEdge edge)
    {
        Splice(edge, edge.prev);
        Splice(edge.sym, edge.sym.prev);
    }

    public static QuarterEdge InsertPoint(QuarterEdge polygonEdge, Vertex point)
    {
        QuadEdge quadEdge = new(new Vertex(polygonEdge.data, polygonEdge.name[0].ToString()), point);
        QuarterEdge firstSpoke = quadEdge.FirstQuarterEdge;

        Splice(firstSpoke, polygonEdge);
        QuarterEdge spoke = firstSpoke;
        int count = 0;
        do
        {
            spoke = ConnectEdges(polygonEdge, spoke.sym);
            polygonEdge = spoke.prev;
            Debug.Log(polygonEdge.name);
            count++;
        } while(count < 1 && polygonEdge.lnext != firstSpoke);

        return firstSpoke;
    }

    public static void FlipEdge(QuarterEdge edge)
    {
        QuarterEdge edge1 = edge.prev;
        QuarterEdge edge2 = edge.sym.prev;

        Splice(edge, edge1);
        Splice(edge.sym, edge2);
        Splice(edge, edge1.lnext);
        Splice(edge.sym, edge2.lnext);

        edge.data = edge1.dest;
        edge.dest = edge2.dest;
    }
}
