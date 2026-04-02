using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewEditor : Editor 
{
	private static readonly List<int> inRange = new();
	private static readonly List<int> inCone = new();
	private static readonly List<int> inSight = new();

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

		LabelTargetsInRadius(fov);
	}

	private void LabelTargetsInRadius(FieldOfView fov)
	{
		inRange.Clear();
		inCone.Clear();
		inSight.Clear();
		
		foreach(Collider2D c in fov.targetsInViewRadius)
		{
			if(c == null) continue;
			
			Vector3 currPos = fov.transform.position;
			Transform t = c.transform;

			bool isPlayer = t.CompareTag("Player");

			Vector2 dirToTarget = (t.position - currPos).normalized;
			bool isInCone = isPlayer && Vector2.Angle(fov.transform.up, dirToTarget) < fov.viewAngle / 2;

			float distToTarget = Vector2.Distance(currPos, t.position);
			RaycastHit2D hit = Physics2D.Raycast(currPos + fov.transform.up, dirToTarget, distToTarget);
			bool isHit = isInCone && hit && hit.transform.CompareTag("Player");

			if(isPlayer)
				AddPlayerToList(t, isHit, isInCone);		
			
			Handles.color = GetColor(isHit, isInCone);
			Handles.DrawSolidDisc(c.transform.position, Vector3.forward, 0.25f);
		}
		
		HighlightPlayers();	
	}

	private void HighlightPlayers()
	{
		if(inRange.Count > 0)
			Handles.DrawOutline(inRange.ToArray(), System.Array.Empty<int>(), Color.blue, Color.clear, 0.5f);

		if(inCone.Count > 0)
			Handles.DrawOutline(inCone.ToArray(), System.Array.Empty<int>(), Color.yellow, Color.clear, 0.5f);
		
		if(inSight.Count > 0)
			Handles.DrawOutline(inSight.ToArray(), System.Array.Empty<int>(), Color.red, Color.clear, 0.5f);
	}

	private void AddPlayerToList(Transform t, bool isHit, bool isInCone)
	{
		if(t.CompareTag("Player"))
		{
			if(t.parent.parent.TryGetComponent<SpriteRenderer>(out var sr))
			{
				if(isHit)
				{
					inSight.Add(sr.GetInstanceID());
				}
				else if(isInCone)
				{
					inCone.Add(sr.GetInstanceID());
				}
				else
				{
					inRange.Add(sr.GetInstanceID());
				}
			}
				
		}
	}

	private Color GetColor(bool isHit, bool isInCone)
	{
		if(isHit) return Color.red;
		else return isInCone ? Color.yellow : Color.blue;
	}
}