using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class BLPEWindowAssist
{
    private static Color _leafColor = new (0, 0.5f, 1);
    private static Color _branchColor = new (0, 1, 0.5f);
    private static Color _recoveryColor = new (0.5f, 0.5f, 0.5f, 0.75f);
    private static Color _highlight = new (1, 1, 0.75f);
    private static Color _panelColor = Color.white;

    private static float _borderRadius = 10f;
    private static float _panelRadius = 3f;
    private static float _arrowStemThickness = 5;
    private static float _arrowHeadSize = 10;

    private static float _panelPad = 10;
    private static float _panelWidth = 175;

    private static int _titleTxtSize = 14, _defaultTxtSize = 12;

    private static GUIStyle _panelTitleTxt;
    public static GUIStyle PanelTitleTxt => _panelTitleTxt ??= CreateCenteredTextStyle();

    private static GUIStyle CreateCenteredTextStyle()
    {
        return new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = _titleTxtSize,
            normal  = { textColor = Color.black },
            hover   = { textColor = Color.black },
        };
    }

    private static GUIStyle _panelLabelTxt;
    public static GUIStyle PanelLabelTxt => _panelLabelTxt ??= CreateLabelTextStyle();

    private static GUIStyle CreateLabelTextStyle()
    {
        return new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
            fontStyle = FontStyle.Bold,
            fontSize = _defaultTxtSize,
            normal  = { textColor = Color.black },
            hover   = { textColor = Color.black },
        };
    }

    private static GUIStyle _panelValueTxt;
    public static GUIStyle PanelValueTxt => _panelValueTxt ??= CreateValueTextStyle();

    private static GUIStyle CreateValueTextStyle()
    {
        return new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleRight,
            fontStyle = FontStyle.Normal,
            fontSize = _defaultTxtSize,
            normal  = { textColor = Color.blue },
            hover   = { textColor = Color.blue },
        };
    }

    private static Dictionary<BehaviorState, float> _recoveringStates = new();
    
    public static void DrawNode(Rect rect, string nodeName, bool isBranch, bool isInPath, GUIStyle style)
    {
        Color nodeBaseColor = isBranch ? _branchColor : _leafColor;
        Color nodeFinalColor = isInPath ? nodeBaseColor + _highlight : nodeBaseColor;
        GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, nodeFinalColor, 0, _borderRadius);
        GUI.Label(rect, CleanName(nodeName), style);
    }

    public static void DrawNodeRecovery(Rect rect, BehaviorState state)
    {
        if(state.IsCoolingDown)
        {
            if(!_recoveringStates.ContainsKey(state)) _recoveringStates[state] = Time.time;
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, _recoveryColor, 0, _borderRadius);
        }
        else if(!state.IsCoolingDown && _recoveringStates.ContainsKey(state)) _recoveringStates.Remove(state);
        
    }

    public static void DrawParentBranch(Rect parentRect, float midX, bool isPath)
    {
        Handles.BeginGUI();
        Handles.color = isPath ? Color.white + _highlight : Color.white;

        Vector2 exit = new(parentRect.xMax, parentRect.center.y);
        Vector2 mid  = new(midX, parentRect.center.y);
        Handles.DrawAAPolyLine(_arrowStemThickness, exit, mid);

        Handles.EndGUI();
    }

    public static void DrawVertBranch(bool isPath, float midX, float vTopY, float vMidY, float vBotY)
    {
        Handles.BeginGUI();
        Handles.color = isPath ? Color.white + _highlight : Color.white;
        
        Vector2 vTop = new(midX, vTopY);
        Vector2 vSecond = !isPath ? new(midX, vBotY) : new(midX, vMidY);

        Handles.DrawAAPolyLine(_arrowStemThickness, vTop, vSecond);

        if(isPath)
        {
            Handles.color = Color.white;

            Vector2 vBot = new(midX, vBotY);
            Handles.DrawAAPolyLine(_arrowStemThickness, vSecond, vBot);
        }

        Handles.EndGUI();
    }

    public static void DrawChildStems((Rect rect, bool isPath) childData, float midX)
    {
        Handles.BeginGUI();

        Handles.color = childData.isPath ? Color.white + _highlight : Color.white;
        Rect rect = childData.rect;
            
        Vector2 bend = new(midX, rect.center.y);
        Vector2 childIn = new(rect.xMin, rect.center.y);
        Handles.DrawAAPolyLine(_arrowStemThickness, bend, childIn);
        DrawArrowHead(childIn);

        Handles.EndGUI();
    }
    
    private static void DrawArrowHead(Vector2 tip)
    {
        Vector2 direction = Vector2.right;
        Vector2 right = new(-direction.y, direction.x);
        Vector2 left  = new( direction.y, -direction.x);

        Vector3[] verts =
        {
            tip,
            tip - direction * _arrowHeadSize + 0.5f * _arrowHeadSize * right,
            tip - direction * _arrowHeadSize + 0.5f * _arrowHeadSize * left,
        };

        Handles.DrawAAConvexPolygon(verts);
    }

    public static List<(string name, object value)> GetConditionals(BehaviorState node)
    {
        List<(string name, object value)> results = new();

        if(node != null)
        {
            Type nodeType = node.GetType(); // Gets the actual child class type, not BaseNode

            // Get all fields (including private ones with BindingFlags)
            PropertyInfo[] properties = nodeType.GetProperties(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            );

            foreach (PropertyInfo prop in properties)
            {
                if (prop.IsDefined(typeof(BranchConditionAttribute), inherit: true))
                    results.Add((prop.Name, prop.GetValue(node)));
            }   
        }

        return results;
    }

    public static void DrawBranchConditionPanel(BehaviorState node, float windowWidth, float windowHeight)
    {
        List<(string name, object value)> props = GetConditionals(node);

        if(props.Count == 0) return;

        float panelHeight = _panelWidth * Mathf.Max(props.Count / 3, 1.25f);
        Rect panelBounds = new (windowWidth - _panelWidth - _panelPad, windowHeight / 2 - panelHeight / 2, _panelWidth, panelHeight);

        GUI.DrawTexture(panelBounds, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, _panelColor, 0, _panelRadius);
        IterateProperties(panelBounds, $"Props of {CleanName(node.name)}", props);
    }

    private static void IterateProperties(Rect bounds, string title, List<(string name, object value)> props)
    {
        float pad = 2f;
        float lineHeight = _defaultTxtSize + pad * 3;

        float x = bounds.xMin + pad;
        float y = bounds.yMin + pad;
        float usableWidth = bounds.width - pad * 2;

        GUI.Label(new Rect(x, y, usableWidth, _titleTxtSize + pad * 4), title, PanelTitleTxt);
        y += _titleTxtSize + pad * 2;

        foreach ((string label, object value) in props)
        {
            string valueStr = value.ToString();

            float labelWidth = PanelLabelTxt.CalcSize(new GUIContent(label)).x;
            float valueWidth = PanelValueTxt.CalcSize(new GUIContent(valueStr)).x;

            float safeLabel = Mathf.Min(labelWidth, usableWidth - valueWidth - pad);
            float excess = Mathf.Max(0, usableWidth - safeLabel - valueWidth - pad);
            float rightLock = safeLabel + excess;
            float valueX = x + rightLock + pad;

            GUI.Label(new Rect(x, y, safeLabel, lineHeight), label, PanelLabelTxt);
            GUI.Label(new Rect(valueX, y, valueWidth, lineHeight), valueStr, PanelValueTxt);

            y += lineHeight + pad;
        }
    }

    private static string CleanName(string name) => name.CompareTo("Enemy Behavior Base") == 0 ? "Base" : name[..name.IndexOf("State")];
}
