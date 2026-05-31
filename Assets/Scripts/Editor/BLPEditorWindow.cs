using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common.TreeGrouper;
using UnityEditor;
using UnityEngine;

public class BLPEditorWindow : EditorWindow
{
    private static GUIStyle _centeredTextStyle;
    private Vector2 _nodeScale = new (100, 50);
    
    private bool _isDirty = false;

    private float x, y, hIncrement, vIncrement;
    private List<BehaviorState> _currentPath = new();
    private List<BehaviorState> _cachedPath = new();
    private List<(Rect rect, BehaviorState state)> _nodeMap = new();
    private BehaviorState _currentNode;

    private float zoomScale = 1.0f;
    private Vector2 _panOffset;
    
    [MenuItem("Window/Decision Tree Flow")]
    public static void ShowWindow()
    {
        GetWindow<BLPEditorWindow>("Decision Tree");
    }

    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnEditorUpdate()
    {
        if (_isDirty && EditorApplication.isPlaying)
        {
            _isDirty = false;
            Repaint();
        }
    }

    private void OnGUI()
    {
        InitializeFlowMap();

        GameObject selected = Selection.activeGameObject;
        if(selected != null && selected.TryGetComponent(out BehaviorLaunchPoint blp))
        {
            BehaviorState baseNode = blp.BehaviorBase;
            MapPath((BehaviorContext)baseNode);
            _isDirty = !CompareToCachedPath();
            ScrollZoomWindow((BehaviorContext)baseNode);
        }
        SelectNode();
        BLPEWindowAssist.DrawBranchConditionPanel(_currentNode, position.width, position.height);

        _currentPath.Clear();
    }

    private void InitializeFlowMap()
    {
        InitializeTextStyle();
        InitializeNodePlotter();
    }
    
    private void InitializeTextStyle()
    {
        _centeredTextStyle ??= new (GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
        };
        _centeredTextStyle.normal.textColor = Color.black;
        _centeredTextStyle.hover.textColor = Color.black;
    }

    private void InitializeNodePlotter()
    {
        x = 10;
        y = 10;

        hIncrement = _nodeScale.x * 1.5f;
        vIncrement = _nodeScale.y * 1.25f;
    }

    private void MapPath(BehaviorContext baseNode)
    {
       if(baseNode == null) return;

        _currentPath.Add(baseNode);

        BehaviorContext context = baseNode;
        while(context != null)
        {
            BehaviorState state = context.CurrentState;
            _currentPath.Add(state);

            context = state is BehaviorContext c ? c : null;
        }
    }

    private bool CompareToCachedPath()
    {
        if(_cachedPath.Count != _currentPath.Count || _cachedPath.Count == 0) return false;
        
        for(int i = 0; i < _cachedPath.Count; i++)
        {
            if(_cachedPath[i] != _currentPath[i]) return false;
        }

        return true;
    }

    private void TraverseTree(BehaviorState state)
    {
        if (state == null) return;

        if (state is BehaviorContext context)
        {
            TraverseChildren(context);
            return;
        }
        Rect rect = GetCurrentNodeRect();
        BLPEWindowAssist.DrawNode(rect, state.name, false, _currentPath.Contains(state), _centeredTextStyle);
        BLPEWindowAssist.DrawNodeRecovery(rect, state);

        _nodeMap.Add((rect, state));
    }

    private void TraverseChildren(BehaviorContext context)
    {
        bool isPath = _currentPath.Contains(context);
        Rect parentRect = GetCurrentNodeRect();
        BLPEWindowAssist.DrawNode(parentRect, context.name, true, isPath, _centeredTextStyle);
        BLPEWindowAssist.DrawNodeRecovery(parentRect, context);
        _nodeMap.Add((parentRect, context));
        x += hIncrement;

        float midX = parentRect.xMax + (GetCurrentNodeRect().xMin - parentRect.xMax) * 0.5f;

        BLPEWindowAssist.DrawParentBranch(parentRect, midX, isPath);

        float vertTop = parentRect.center.y;
        float vertMid = parentRect.center.y;
        float vertBot = parentRect.center.y;

        foreach (var entry in context.PossibleStates)
        {
            bool isChildPath = _currentPath.Contains(entry.State);
            Rect childRect = GetCurrentNodeRect();
            if(isChildPath) vertMid = childRect.center.y;
            vertBot = childRect.center.y;
            BLPEWindowAssist.DrawChildStems((childRect, isChildPath), midX);
            TraverseTree(entry.State);
            y += vIncrement;
        }
        
        BLPEWindowAssist.DrawVertBranch(isPath, midX, vertTop, vertMid, vertBot);
        x -= hIncrement;
        y -= vIncrement;
    }

    private Rect GetCurrentNodeRect() => new(x, y, _nodeScale.x, _nodeScale.y);


    private void ScrollZoomWindow(BehaviorContext baseNode)
    {
        Event e = Event.current;

        if (e.type == EventType.ScrollWheel)
        {
            float zoomDelta = e.delta.y > 0 ? -0.1f : 0.1f;
            zoomScale = Mathf.Clamp(zoomScale + zoomDelta, 0.7f, 5.0f);
            e.Use();
        }

        Matrix4x4 oldMatrix = GUI.matrix;
        
        Vector2 pivot = Event.current.mousePosition; 
        GUI.matrix = Matrix4x4.TRS(pivot, Quaternion.identity, Vector3.one) * 
            Matrix4x4.Scale(new Vector3(zoomScale, zoomScale, 1.0f)) * 
            Matrix4x4.TRS(-pivot, Quaternion.identity, Vector3.one);

        PanWindow(baseNode);
        

        GUI.matrix = oldMatrix;
    }

    private void PanWindow(BehaviorContext baseNode)
    {
        Event e = Event.current;
        
        if (e.type == EventType.MouseDrag && e.button == 2)
        {
            _panOffset += e.delta;
            e.Use();
            Repaint();
        }

        BeginWindows();
            GUILayout.BeginArea(new Rect(_panOffset.x, _panOffset.y, 1000, 1000));
                TraverseTree(baseNode);
            GUILayout.EndArea();
        EndWindows();
    }

    private void SelectNode()
    {
        Event e = Event.current;

        if(e.type == EventType.MouseDown && e.button == 0)
        {
            Vector2 mousePos = e.mousePosition;
            foreach((Rect rect, BehaviorState state) in _nodeMap)
            {
                if(rect.Contains(mousePos))
                {
                    _currentNode = state;
                    return;
                }
            }

            _currentNode = null;
        }
    }
}
