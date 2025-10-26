using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewEditor : Editor {

	void OnSceneGUI() {
		FieldOfView fov = (FieldOfView)target;
		Handles.color = Color.blue;
		Handles.DrawWireArc (fov.transform.position, Vector3.forward, Vector2.up, 360, fov.viewRadius, 2);
		Vector2 viewAngleA = fov.DirFromAngle (-fov.viewAngle / 2);
		Vector2 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2);

		Handles.DrawLine (fov.transform.position, (Vector2)fov.transform.position + viewAngleA * fov.viewRadius);
		Handles.DrawLine (fov.transform.position, (Vector2)fov.transform.position + viewAngleB * fov.viewRadius);

		Handles.color = Color.red;
		foreach (Transform visibleTarget in fov.visibleTargets) {
			Handles.DrawLine (fov.transform.position, visibleTarget.position);
		}
	}
}