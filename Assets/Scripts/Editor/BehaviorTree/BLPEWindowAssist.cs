using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

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
    private static float _symbolRadius = 6f;
    private static float _symbolThickness = 6f;
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
    
    public static void DisplayCurrentTimestamp()
    {
        string label = "Last Repaint: " + Time.time.ToString("F2");
        Vector2 timeSize = PanelValueTxt.CalcSize(new GUIContent(label));
        Rect rect = new(0, 0, timeSize.x, timeSize.y);
        GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, Color.white, 0, _panelRadius);
        GUI.Label(rect, label, PanelValueTxt);
    }
    
    public static void DrawNode(Rect rect, string nodeName, float timestamp, bool isBranch, bool isInPath, GUIStyle style)
    {
        Color nodeBaseColor = isBranch ? _branchColor : _leafColor;
        Color nodeFinalColor = isInPath ? nodeBaseColor + _highlight : nodeBaseColor;
        GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, nodeFinalColor, 0, _borderRadius);
        
        string label = CleanName(nodeName);
        Vector2 labelSize = style.CalcSize(new GUIContent(label));
        Vector2 labelOrigin = new(rect.center.x - labelSize.x / 2, rect.center.y - labelSize.y / 2);
        Rect labelRect = new(labelOrigin, labelSize);
        GUI.Label(labelRect, label, style);

        string timeStr = timestamp.ToString("F2");
        Vector2 timeSize = PanelLabelTxt.CalcSize(new GUIContent(timeStr));
        Vector2 timeOrigin = new(rect.center.x - timeSize.x / 2, rect.center.y + labelSize.y / 2 - timeSize.y / 2 + 4);
        Rect timeRect = new(timeOrigin.x, timeOrigin.y, timeSize.x, timeSize.y);
        GUI.Label(timeRect, timeStr, PanelLabelTxt);
    }

    public static void DrawNodeRecovery(Rect rect, BehaviorState state)
    {
        if(state.IsCoolingDown)
        {
            if(!_recoveringStates.ContainsKey(state)) _recoveringStates[state] = Time.time;
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, _recoveryColor, 0, _borderRadius);
        }
        else if(!state.IsCoolingDown && _recoveringStates.ContainsKey(state)) _recoveringStates.Remove(state);

        if(state is BehaviorContext context)
        {
            if(context.branchValidity.Count == 0)
            {
                context.InitializeBranchValidityRec();
            }
            else
            {
                context.UpdateBranchValidityRec();
            }
        }
        
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

    public static void DrawVertBranch(int lastWeight, bool isPath, float midX, float vTopY, float vMidY, float vBotY)
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
        LabelLastWeightedRoll(lastWeight, vTop);

        Handles.EndGUI();
    }

    public static void DrawChildStem(BehaviorContext parent, IBehaviorContext.WeightedState child, (Rect rect, bool isPath) childData, float midX)
    {
        Handles.BeginGUI();

        Handles.color = childData.isPath ? Color.white + _highlight : Color.white;
        Rect rect = childData.rect;
            
        Vector2 bend = new(midX, rect.center.y);
        Vector2 childIn = new(rect.xMin, rect.center.y);
        Handles.DrawAAPolyLine(_arrowStemThickness, bend, childIn);
        DrawArrowHead(childIn);
        DrawPathValiditySymbol(parent, child.State, childIn + (_arrowHeadSize + _symbolRadius) * Vector2.left);
        LabelBranchWeight(child.Weight, childIn + (_arrowHeadSize + _symbolRadius) * Vector2.left);

        Handles.EndGUI();
    }

    private static void LabelLastWeightedRoll(int weight, Vector2 drawPoint)
    {
        if(weight > 0)
        {
            float pad = 2f;
            float lineHeight = _titleTxtSize + pad * 3;
            float labelWidth = PanelTitleTxt.CalcSize(new GUIContent(weight.ToString())).x;
            
            Rect rect = new(drawPoint.x - labelWidth / 2, drawPoint.y - lineHeight / 2, labelWidth, lineHeight);
            DrawDisk(rect.center, rect.width / 2, Color.cyan);
            GUI.Label(rect, weight.ToString(), PanelTitleTxt);
        }
    }
    
    private static void LabelBranchWeight(int weight, Vector2 rightCap)
    {
        if(weight > 0)
        {
            float pad = 2f;
            float lineHeight = _titleTxtSize + pad * 3;
            float labelWidth = PanelTitleTxt.CalcSize(new GUIContent(weight.ToString())).x;
            
            Vector2 drawPoint = rightCap + labelWidth * Vector2.left;
            Rect rect = new(drawPoint.x - labelWidth, drawPoint.y - lineHeight / 2, labelWidth, lineHeight);
            DrawDisk(rect.center, rect.width / 2, Color.white);
            GUI.Label(rect, weight.ToString(), PanelTitleTxt);
        }
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

    private static void DrawPathValiditySymbol(BehaviorContext parent, BehaviorState child, Vector2 drawPoint)
    {
        if(!parent.branchValidity.ContainsKey(child)) return;

        bool isValid = parent.branchValidity[child];

        if(isValid) DrawO(drawPoint); else DrawX(drawPoint);
        
        //TimestampLabel(parent.branchValidity[child].Timestamp,  drawPoint - _symbolRadius * 3 * Vector2.down);
    }

    /*private static void TimestampLabel(float time, Vector2 drawPoint)
    {
        string timeStr = time.ToString("F2");
        float pad = 2f;
        float lineHeight = _defaultTxtSize + pad * 3;
        float labelWidth = PanelValueTxt.CalcSize(new GUIContent(timeStr)).x;
        Rect rect = new(drawPoint.x - labelWidth / 2, drawPoint.y - lineHeight / 2, labelWidth, lineHeight);
        
        GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, Color.white, 0, _panelRadius);
        GUI.Label(rect, timeStr, PanelValueTxt);
    }*/

    private static void DrawX(Vector2 drawPoint)
    {
        Vector2 topLeft = new (drawPoint.x - _symbolRadius, drawPoint.y + _symbolRadius);
        Vector2 topRight = new (drawPoint.x + _symbolRadius, drawPoint.y + _symbolRadius);
        Vector2 botLeft = new (drawPoint.x - _symbolRadius, drawPoint.y - _symbolRadius);
        Vector2 botRight = new (drawPoint.x + _symbolRadius, drawPoint.y - _symbolRadius); 

        Handles.color = Color.red;
        Handles.DrawAAPolyLine(_symbolThickness, new Vector3[]{ topLeft, botRight});
        Handles.DrawAAPolyLine(_symbolThickness, new Vector3[]{ botLeft, topRight});
    }

    private static void DrawO(Vector2 drawPoint)
    {
        int segments = 32;
        Vector3[] points = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            points[i] = drawPoint + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _symbolRadius;
        }

        Handles.color = Color.green;
        Handles.DrawAAPolyLine(_symbolThickness, points);
    }

    private static void DrawDisk(Vector2 drawPoint, float radius, Color color)
    {
        Color cachedColor = Handles.color;
        Handles.color = color;
        Vector3[] points = new Vector3[32];
        for (int i = 0; i < 32; i++)
        {
            float angle = i * Mathf.PI * 2f / 32;
            points[i] = drawPoint + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }
        Handles.DrawAAConvexPolygon(points);
        Handles.color = cachedColor;
    }

    public static List<(string name, object value)> GetAttributes(BehaviorState node, Type type)
    {
        List<(string name, object value)> results = new();

        if(node != null)
        {
            Type nodeType = node.GetType();

            PropertyInfo[] properties = nodeType.GetProperties
            (
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            );

            foreach (PropertyInfo prop in properties)
            {
                if (prop.IsDefined(type, inherit: true))
                    results.Add((prop.Name, prop.GetValue(node)));
            }   
        }

        return results;
    }

    public static void DrawBranchConditionPanel(BehaviorState node, float windowWidth, float windowHeight)
    {
        List<(string name, object value)> conditionals = GetAttributes(node, typeof(BranchConditionAttribute));
        List<(string name, object value)> timestamps = GetAttributes(node, typeof(ConditionTimestampAttribute));

        List<(string name, bool result, float time)> branchResults = new();
        for(int i = 0; i < conditionals.Count; i++)
        {
            (string name, object value) = conditionals[i];
            bool result = (bool)value;
            float timestamp = (float)timestamps[i].value;
            
            branchResults.Add((name, result, timestamp));
        }

        if(conditionals.Count == 0) return;

        float panelHeight = _panelWidth * Mathf.Max(conditionals.Count / 3, 1.25f);
        Rect panelBounds = new (windowWidth - _panelWidth - _panelPad, windowHeight / 2 - panelHeight / 2, _panelWidth, panelHeight);

        GUI.DrawTexture(panelBounds, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, _panelColor, 0, _panelRadius);
        IterateProperties(panelBounds, $"Props of {CleanName(node.name)}", branchResults);
    }

    private static void IterateProperties(Rect bounds, string title, List<(string name, bool result, float time)> props)
    {
        float pad = 2f;
        float lineHeight = _defaultTxtSize + pad * 3;

        float x = bounds.xMin + pad;
        float y = bounds.yMin + pad;
        float usableWidth = bounds.width - pad * 2;

        GUI.Label(new Rect(x, y, usableWidth, _titleTxtSize + pad * 4), title, PanelTitleTxt);
        y += _titleTxtSize + pad * 2;

        foreach ((string label, bool result, float time) in props)
        {
            string resultStr = result.ToString();
            string timeStr = time.ToString("F2");

            float labelWidth = PanelLabelTxt.CalcSize(new GUIContent(label)).x;
            float resultWidth = PanelValueTxt.CalcSize(new GUIContent(resultStr)).x;
            float timeWidth = PanelLabelTxt.CalcSize(new GUIContent(timeStr)).x;

            float safeLabel = Mathf.Min(labelWidth, usableWidth - resultWidth - timeWidth - pad);
            float excess = Mathf.Max(0, usableWidth - safeLabel - resultWidth - timeWidth - pad * 2);
            float rightLock = safeLabel + resultWidth + excess;
            float x2 = x + safeLabel + pad;
            float x3 = x + rightLock + pad;

            GUI.Label(new Rect(x, y, safeLabel, lineHeight), label, PanelLabelTxt);
            GUI.Label(new Rect(x2, y, resultWidth, lineHeight), resultStr, PanelValueTxt);
            GUI.Label(new Rect(x3, y, timeWidth, lineHeight), timeStr, PanelValueTxt);

            y += lineHeight + pad;
        }
    }

    private static string CleanName(string name) => name.CompareTo("Enemy Behavior Base") == 0 ? "Base" : name[..name.IndexOf("State")];
}
