using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    LineRenderer lr;
    GameObject line;

    // Start is called before the first frame update
    void Start()
    {
        BuildLine();
    }

    private void BuildLine()
    {
        Vector3[] poses = {new Vector3(0, 0, 0), new Vector3(3, 0, 0)};
        line = new GameObject();
        line.transform.position = (poses[0] - poses[1]).normalized * (Vector3.Distance(poses[0], poses[1]) / 2);
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.SetPositions(poses);
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.startColor = Color.red;
        lr.endColor = Color.white;
    } 
}
