using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Affector), true)]
public class AffectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        AffectorDrawerAssist.InitHandlers();
        EditorGUI.BeginProperty(position, label, property);

        float y = position.y;
        float lineH = EditorGUIUtility.singleLineHeight;
        float pad = 2f;

        // Draw the foldout label for the Effect object itself
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, y, position.width, lineH), property.isExpanded, label, true);
        y += lineH + pad;

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            TargetCoordinatorContext.SuppressCoordinatorPanel = false;

            // Iterate all direct children, handle lifetime fields specially
            SerializedProperty iter = property.Copy();
            SerializedProperty end = property.GetEndProperty();
            iter.NextVisible(true); // move into first child

            while (!SerializedProperty.EqualContents(iter, end))
            {
                float drawnHeight;
                if (AffectorDrawerAssist.handlerMap.TryGetValue(iter.name, out AffectorDrawerAssist.PropertyHandler handler))
                {
                    drawnHeight = handler(new Rect(position.x, y, position.width, 0), property, iter.Copy());
                }
                else
                {
                    // Draw all other fields (effectName, stat, etc.) normally
                    drawnHeight = EditorGUI.GetPropertyHeight(iter, true);
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, drawnHeight), iter, true);
                }

                if(drawnHeight > 0f) y += drawnHeight + pad;

                iter.NextVisible(false);
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
        TargetCoordinatorContext.Reset();
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

        while (!SerializedProperty.EqualContents(iter, end))
        {
            float h;
            if (AffectorDrawerAssist.references.Contains(iter.name))
            {
                h = iter.managedReferenceValue == null
                    ? 0f
                    : EditorGUI.GetPropertyHeight(iter, true);
            }
            else
            {
                h = EditorGUI.GetPropertyHeight(iter, true);
            }

            if (h > 0f) total += h + pad;
            iter.NextVisible(false);
        }

        return total;
    }
}