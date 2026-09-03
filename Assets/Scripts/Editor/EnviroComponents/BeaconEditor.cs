using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (Beacon))]
public class BeaconEditor : Editor
{
    void OnSceneGUI() {
		Beacon beacon = (Beacon)target;
		Handles.color = Color.yellow;
		Handles.DrawWireArc (beacon.transform.position, Vector3.forward, Vector2.up, 360, beacon.radius, 2);
	}
}
