using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Effect), true)]
public class EffectDrawer : PropertyDrawer
{
    private delegate float PropertyHandler(Rect position, SerializedProperty property, SerializedProperty element);
    private Dictionary<string, PropertyHandler> handlerMap;
    private string[] references =
    {
      "lifetime",
      "runs"
    };
    

    private void InitHandlers()
    {
        if (handlerMap != null) return;
        handlerMap = new Dictionary<string, PropertyHandler>
        {
            { "lifetimeLabel", HandleLabelField("lifetime", UpdateLifetimeInstance) },
            { "lifetime", HandleManagedReference() },
            { "repeatLabel", HandleLabelField("runs", UpdateRepeatInstance) },
            { "runs", HandleManagedReference() },
            { "targetLabel", UpdateCoordinatorGUI("coordinator") },
            { "coordinator", HandleFoldableProp() },
        };
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        InitHandlers();
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
                if (handlerMap.TryGetValue(iter.name, out PropertyHandler handler))
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

    private void UpdateLifetimeInstance(SerializedProperty labelProp, SerializedProperty lifeProp)
    {
        Effect.LifetimeLabel selected = (Effect.LifetimeLabel)labelProp.enumValueIndex;
        
        lifeProp.managedReferenceValue = selected switch
        {
            Effect.LifetimeLabel.instant => new LifetimeLogic(),
            Effect.LifetimeLabel.limited => new LimitedLifetime(),
            Effect.LifetimeLabel.conditional => new ConditionalLifetime(),
            _ => null
        };
        
        labelProp.serializedObject.ApplyModifiedProperties();
    }

    private void UpdateRepeatInstance(SerializedProperty labelProp, SerializedProperty repeatProp)
    {
        Effect.RepeatLabel selected = (Effect.RepeatLabel)labelProp.enumValueIndex;
        
        repeatProp.managedReferenceValue = selected switch
        {
            Effect.RepeatLabel.once => new RepeatLogic(),
            Effect.RepeatLabel.continunous => new ContinuousRepeat(),
            Effect.RepeatLabel.iterate => new IterateRepeat(),
            Effect.RepeatLabel.period => new PeriodRepeat(),
            Effect.RepeatLabel.persist => new PersistRepeat(),
            _ => null
        };
        
        labelProp.serializedObject.ApplyModifiedProperties();
    }

    private PropertyHandler HandleLabelField(string siblingName, Action<SerializedProperty, SerializedProperty> updater)
    {
        return (position, property, element) =>
        {
            float lineH = EditorGUIUtility.singleLineHeight;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, lineH), element);

            if (EditorGUI.EndChangeCheck())
            {
                SerializedProperty sibling = property.FindPropertyRelative(siblingName);
                updater(element, sibling);
            }
            return lineH;
        };
    }

    private PropertyHandler HandleManagedReference()
    {
        return (position, property, element) =>
        {
            if (element.managedReferenceValue == null || !element.hasVisibleChildren) return 0f;

            float h = EditorGUI.GetPropertyHeight(element, true);
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, h), element, true);
            return h;
        };
    }

    private PropertyHandler HandleFoldableProp()
    {
        return (position, property, element) =>
        {
            float h = EditorGUI.GetPropertyHeight(element, true);
            if(h <= EditorGUIUtility.singleLineHeight) return 0f;
            
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, h), element, true);
            return h;
        };
    }

    private PropertyHandler UpdateCoordinatorGUI(string siblingName)
    {
        return (position, property, element) =>
        {
            float lineH = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, lineH), element);

            SerializedProperty sibling = property.FindPropertyRelative(siblingName);
            Effect.TargetLabel selected = (Effect.TargetLabel)element.enumValueIndex;
            
            if(selected == Effect.TargetLabel.self) TargetCoordinatorContext.SuppressCoordinatorPanel = true;
            else if(selected == Effect.TargetLabel.environment) TargetCoordinatorContext.SuppressAreaConfig = true;
            
            return lineH;
        };
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
            if (references.Contains(iter.name))
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