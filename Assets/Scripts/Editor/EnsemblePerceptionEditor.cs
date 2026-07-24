using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

//[CustomEditor(typeof(EnsemblePerception))]
public class EnsemblePerceptionEditor : Editor
{
    /*private EnsemblePerception perception;
    private EntityProperties props;
    private LayerMask checkForCollision;
    private NavMeshPath oldPath;
    private NavMeshPath newPath;
    private float pulse;

    private void OnSceneGUI()
    {
        if(perception == null)
        {
            perception = (EnsemblePerception)target;
            props = perception.EntityProps;
            checkForCollision = perception.CheckForCollision;
            oldPath = perception.OldPath;
            newPath = perception.NewPath;
        }
        
        if(props != null)
        {
            Vector2 pos = props.NavMeshAgent.steeringTarget;
            
            if(perception.IsSearchingRange)
                DrawFullRange(pos);
            DrawRegion(pos);
            
            Pulse(pos);
            //DrawPaths();
        }
    }

    private void DrawRegion(Vector2 pos)
    {
        Rect regionCheck = new(pos.x - 1.5f, pos.y - 1.5f, 3, 3);
        Vector3[] points = 
        { 
            new(regionCheck.xMin, regionCheck.yMin), 
            new(regionCheck.xMax, regionCheck.yMin), 
            new(regionCheck.xMax, regionCheck.yMax), 
            new(regionCheck.xMin, regionCheck.yMax), 
            new(regionCheck.xMin, regionCheck.yMin)
        };

        Handles.color = Color.yellow;
        Handles.DrawAAPolyLine(points);
    }

    private void DrawFullRange(Vector2 pos)
    {
        Rect rangeCheck = new(pos.x - 4.5f, pos.y - 4.5f, 9, 9);
        Vector3[] pointsOutline =
        {
            new(rangeCheck.xMin, rangeCheck.yMin), 
            new(rangeCheck.xMax, rangeCheck.yMin), 
            new(rangeCheck.xMax, rangeCheck.yMax), 
            new(rangeCheck.xMin, rangeCheck.yMax), 
            new(rangeCheck.xMin, rangeCheck.yMin)
        };

        Vector3[] pointsVertA =
        {
            new(rangeCheck.xMin + 3, rangeCheck.yMin),
            new(rangeCheck.xMin + 3, rangeCheck.yMax)
        };
        Vector3[] pointsVertB =
        {
            new(rangeCheck.xMin + 6, rangeCheck.yMin),
            new(rangeCheck.xMin + 6, rangeCheck.yMax)
        };

        Vector3[] pointsHorA =
        {
            new(rangeCheck.xMin, rangeCheck.yMin + 3),
            new(rangeCheck.xMax, rangeCheck.yMin + 3)
        };
        Vector3[] pointsHorB =
        {
            new(rangeCheck.xMin, rangeCheck.yMin + 6),
            new(rangeCheck.xMax, rangeCheck.yMin + 6)
        };

        Handles.color = Color.green;
        Handles.DrawAAPolyLine(pointsOutline);
        Handles.DrawAAPolyLine(pointsVertA);
        Handles.DrawAAPolyLine(pointsVertB);
        Handles.DrawAAPolyLine(pointsHorA);
        Handles.DrawAAPolyLine(pointsHorB);
    }

    private void Pulse(Vector2 pos)
    {
        if(perception.Elapsed > perception.TestInterval || pulse > 0.1f)
        {
            pulse = (Mathf.Sin((float)EditorApplication.timeSinceStartup * 2f) + 1f) / 2f;
            Color pulseColor = new (1, 1, 1, pulse * 0.3f);
            Handles.color = pulseColor;
            Handles.DrawSolidDisc(pos, Vector3.forward, 4.5f);
            PingObjects(pos);

            SceneView.RepaintAll();
        }
        else
        {
            perception.IsSearchingRange = false;
        }
    }


    List<int> inRange = new();
    List<Transform> objs = new();
    private void PingObjects(Vector2 pos)
    {
        inRange.Clear();
        objs.Clear();

        objs = Physics2D.OverlapCircleAll(pos, 4.5f, checkForCollision).Select(c => c.transform).ToList();
        inRange = objs
            .Select(t => t.GetComponent<SpriteRenderer>())
            .Where(sr => sr != null)
            .Select(sr => sr.GetInstanceID())
            .ToList();

        
        bool hasTarget = false;
        Transform target = props.TargetTransform;
        if(target != null)
        {
            hasTarget = objs.Contains(target);
            inRange.Remove(target.GetComponent<SpriteRenderer>().GetInstanceID());
        }
            

        if(inRange.Count > 0)
			Handles.DrawOutline(inRange.ToArray(), System.Array.Empty<int>(), Color.yellow, Color.clear, 0.5f);

        if(hasTarget)
        {
            Handles.DrawOutline(
                new int[]{target.GetComponent<SpriteRenderer>().GetInstanceID()}, 
                System.Array.Empty<int>(), Color.red, Color.clear, 0.5f);
        }
    }

    private void DrawPaths()
    {
        if(newPath != null)
        {
            Handles.color = Color.green;
            Handles.DrawAAPolyLine(5, newPath.corners.ToArray());

            Handles.color = Color.red;
            Handles.DrawAAPolyLine(3, oldPath.corners.ToArray());
        }
    }*/
}
