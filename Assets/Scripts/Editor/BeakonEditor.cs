using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (Beakon))]
public class BeakonEditor : Editor
{
    void OnSceneGUI() {
		Beakon beakon = (Beakon)target;
		Handles.color = Color.yellow;
		Handles.DrawWireArc (beakon.transform.position, Vector3.forward, Vector2.up, 360, beakon.radius, 2);
	}
}
