using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TransformAffecter), true)]
public class TransformAffecterDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        AffecterDrawerAssist.InitHandlers();
        TargetCoordinatorContext.Reset();
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
            
            while (!SerializedProperty.EqualContents(iter, end))
            {
                
                float drawnHeight;
                if (AffecterDrawerAssist.handlerMap.TryGetValue(iter.name, out AffecterDrawerAssist.PropertyHandler handler))
                {
                    drawnHeight = handler(new Rect(position.x, y, position.width, 0), property, iter.Copy());
                }
                else
                {
                    if(ShouldSkipElement(property, iter.name))
                    {
                        iter.NextVisible(false);
                        continue;
                    }

                    if(iter.name == "isInstant" && iter.boolValue)
                    {
                        property.FindPropertyRelative("isKinematic").boolValue = true;
                    }
                    else if(iter.name == "hasTarget" && !iter.boolValue)
                    {
                        property.FindPropertyRelative("isOptimal").boolValue = false;
                    }

                    drawnHeight = EditorGUI.GetPropertyHeight(iter, true);
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, drawnHeight), iter, true);
                }

                if(drawnHeight > 0f) y += drawnHeight + pad;

                iter.NextVisible(false);
            }
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineH = EditorGUIUtility.singleLineHeight;
        float pad = 2f;
        float total = lineH + pad;

        if(!property.isExpanded) return total;

        SerializedProperty iter = property.Copy();
        SerializedProperty end = property.GetEndProperty();
        iter.NextVisible(true);
        
        while (!SerializedProperty.EqualContents(iter, end))
        {
            if(ShouldSkipElement(property, iter.name))
            {
                iter.NextVisible(false);
                continue;
            }

            float h = EditorGUI.GetPropertyHeight(iter, true);

            if(h > 0f) total += h + pad;

            iter.NextVisible(false);
        }
        
        return total;
    }

    private bool ShouldSkipElement(SerializedProperty property, string element)
    {
        TransformAffecter.TransformLabel transformLabel = (TransformAffecter.TransformLabel)property.FindPropertyRelative("transformLabel").enumValueIndex;
        bool isInstant = property.FindPropertyRelative("isInstant").boolValue;
        bool useHostStat = property.FindPropertyRelative("useHostStat").boolValue;
        bool hasTarget = property.FindPropertyRelative("hasTarget").boolValue;
        bool isOptimal = property.FindPropertyRelative("isOptimal").boolValue;
        bool isRotation = transformLabel == TransformAffecter.TransformLabel.rotation;
        bool isPosition = transformLabel == TransformAffecter.TransformLabel.position;
        bool isScale = transformLabel == TransformAffecter.TransformLabel.scale;

        return element switch
        {
            "speed" => isInstant,
            "useHostStat" => isInstant,
            "selectedStat" => isInstant || !useHostStat,
            "hasTarget" => isScale,
            "position" => !hasTarget || !isPosition,
            "angle" => !hasTarget || !isRotation,
            "scale" => isRotation || isPosition,
            "direction" => hasTarget || isRotation || isScale,
            "isOptimal" => !hasTarget,
            "isClockwise" => isOptimal || isPosition || isScale,
            "isKinematic" => isInstant,
            _ => false
        };
    }
}
