using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDelaunayTriangulation : MonoBehaviour
{
    public Vector3[] coords;
    private QuarterEdge selectedQtr;

    // Start is called before the first frame update
    void Start()
    {
        selectedQtr = EdgeManager.MakeTriangle(new Vertex(coords[0], "A"), new Vertex(coords[1], "B"), new Vertex(coords[2], "C"));
        selectedQtr = EdgeManager.InsertPoint(selectedQtr, new Vertex(coords[3], "D"));
        selectedQtr.ToggleLine();
    }

    // Update is called once per frame
    void Update()
    {
        RotateQuarterEdge();
    }

    private void RotateQuarterEdge()
    {
        if(Input.GetKeyUp(KeyCode.W))
        {
            print($"Get opposite of {selectedQtr.name} which is {selectedQtr.sym.name}");
            selectedQtr.ToggleLine();
            selectedQtr = selectedQtr.sym;
            selectedQtr.ToggleLine();
        }
        else if(Input.GetKeyUp(KeyCode.A))
        {
            print($"Get reverse of {selectedQtr.name} which is {selectedQtr.tor.name}");
            selectedQtr.ToggleLine();
            selectedQtr = selectedQtr.tor;
            selectedQtr.ToggleLine();
        }
        else if(Input.GetKeyUp(KeyCode.S))
        {
            print($"Get next of {selectedQtr.name} which is {selectedQtr.next.name}");
            selectedQtr.ToggleLine();
            selectedQtr = selectedQtr.next;
            selectedQtr.ToggleLine();
        }
        else if(Input.GetKeyUp(KeyCode.D))
        {
            print($"Get rotation of {selectedQtr.name} which is {selectedQtr.rot.name}");
            selectedQtr.ToggleLine();
            selectedQtr = selectedQtr.rot;
            selectedQtr.ToggleLine();
        }
    }
}
