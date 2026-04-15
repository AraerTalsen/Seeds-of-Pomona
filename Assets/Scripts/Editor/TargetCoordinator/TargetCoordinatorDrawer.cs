using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TargetCoordinator), true)]
public class TargetCoordinatorDrawer : PropertyDrawer
{
    private float panelHeight = EditorGUIUtility.singleLineHeight * 25;
    private Rect panel;
    private Rect temp;
    private List<Rect> targetAreas = new();
    private Sprite unitCircle = Resources.Load<Sprite>("Sprites/grayUnitCircle");
    private Sprite targetReticle = Resources.Load<Sprite>("Sprites/reticle");
    private Sprite targetUnitCir;
    private Vector2 lastArea = new (0, 0);
    private Vector2 center;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if(!TargetCoordinatorContext.SuppressCoordinatorPanel)
        {
            targetUnitCir = TCPainter.GenerateSprite();
            EditorGUI.BeginProperty(position, label, property);

            float y = position.y;
            float lineH = EditorGUIUtility.singleLineHeight;
            float pad = 2f;

            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, y, position.width, lineH), property.isExpanded, label, true);
            y += lineH + pad;

            if(property.isExpanded)
            {
                EditorGUI.indentLevel++;

                SerializedProperty iter = property.Copy();
                SerializedProperty end = property.GetEndProperty();
                iter.NextVisible(true);

                bool isEnviro = TargetCoordinatorContext.SuppressAreaConfig;
                bool hasBounds = true;
                bool isDynamic = true;
                
                while (!SerializedProperty.EqualContents(iter, end))
                {
                    if(iter.name == "hasBounds") 
                    {
                        if(TargetCoordinatorContext.SuppressAreaConfig)
                        {
                            hasBounds = false;
                            iter.NextVisible(false);
                            continue;
                        }
                        hasBounds = iter.boolValue;
                    }
                    else if(iter.name == "isDynamic")
                    {
                        isDynamic = iter.boolValue;
                    }
                    else if(iter.name == "area" && !hasBounds) 
                    {
                        iter.NextVisible(false);
                        continue;
                    }

                    float drawnHeight = EditorGUI.GetPropertyHeight(iter, true);
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, drawnHeight), iter, true);

                    if(drawnHeight > 0f) y += drawnHeight + pad;

                    iter.NextVisible(false);
                }            

                if((!isEnviro && hasBounds) || (isEnviro && !isDynamic))
                {
                    CreateCoordinationPanel(position, y);
                    TCControls.MouseControls(position, lastArea, out lastArea);
                    UpdateTargetUnits(hasBounds);
                } 
            }

            EditorGUI.EndProperty();
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineH = EditorGUIUtility.singleLineHeight;
        float pad = 2f;
        float total = lineH + pad; // foldout header

        if (!property.isExpanded) return total;

        SerializedProperty iter = property.Copy();
        SerializedProperty end = property.GetEndProperty();
        iter.NextVisible(true);

        bool isEnviro = TargetCoordinatorContext.SuppressAreaConfig;
        bool hasBounds = true;
        bool isDynamic = true;

        if(!TargetCoordinatorContext.SuppressCoordinatorPanel)
        {
            while (!SerializedProperty.EqualContents(iter, end))
            {
                if(iter.name == "area" && !hasBounds) 
                {
                    iter.NextVisible(false);
                    continue;
                }

                float h = EditorGUI.GetPropertyHeight(iter, true);

                if (h > 0f) total += h + pad;

                if(iter.name == "hasBounds") 
                {
                    hasBounds = iter.boolValue;
                }
                else if(iter.name == "isDynamic")
                {
                    isDynamic = iter.boolValue;
                }
                
                iter.NextVisible(false);
            }

            if((!isEnviro && hasBounds) || (isEnviro && !isDynamic)) total += panelHeight + pad;
        }

        return total;
    }

    private void CreateCoordinationPanel(Rect position, float y)
    {
        panel = new (position.x, y, position.width, panelHeight);
        EditorGUI.DrawRect(panel, new Color(0, 0, 0, 0.25f));

        if (Event.current.type == EventType.Repaint)
            center = new (position.x + position.width / 2, y + panelHeight / 2);
            
        Vector2 pos = TCPainter.CenterOnUnit(unitCircle, center);
        GUI.DrawTexture(new Rect(pos.x, pos.y, unitCircle.texture.width, unitCircle.texture.height), unitCircle.texture);
    }

    private void UpdateTargetUnits(bool hasArea)
    {
        Sprite unit = hasArea ? targetUnitCir : targetReticle;
        Vector2 topLeft = TCPainter.CenterOnUnit(unit, lastArea);
        Rect targetPos = new(topLeft.x, topLeft.y, unit.texture.width, unit.texture.height);
        targetPos = TCPainter.ClampRectToPanel(panel, targetPos);
        GUI.DrawTexture(targetPos, unit.texture);
        temp = targetPos;
    }
}
