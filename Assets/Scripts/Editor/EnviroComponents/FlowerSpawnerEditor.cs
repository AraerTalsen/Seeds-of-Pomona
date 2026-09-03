using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(FlowerSpawner))]
public class FlowerSpawnerEditor : Editor
{
    private void OnSceneGUI()
    {
        FlowerSpawner spawner = (FlowerSpawner)target;
        Handles.color = Color.green;

        foreach(FlowerSpawner.BoundBox box in spawner.BoundBoxes)
        {
            (Vector2 topLeft, Vector2 botRight) = box;
            DrawWireQuad(topLeft, botRight);
        }
    }

    private void DrawWireQuad(Vector2 topLeft, Vector2 botRight)
    {
        Vector2 topRight = new (botRight.x, topLeft.y);
        Vector2 botLeft = new (topLeft.x, botRight.y);

        Handles.DrawPolyLine(topLeft, topRight, botRight, botLeft, topLeft);
    }
}
