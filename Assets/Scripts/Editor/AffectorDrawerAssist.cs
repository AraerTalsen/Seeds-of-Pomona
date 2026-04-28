using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AffectorDrawerAssist
{
    public delegate float PropertyHandler(Rect position, SerializedProperty property, SerializedProperty element);
    public static Dictionary<string, PropertyHandler> handlerMap;
    public static string[] references =
    {
      "lifetime",
      "runs"
    };
    

    public static void InitHandlers()
    {
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

    private static void UpdateLifetimeInstance(SerializedProperty labelProp, SerializedProperty lifeProp)
    {
        Affector.LifetimeLabel selected = (Affector.LifetimeLabel)labelProp.enumValueIndex;
        
        lifeProp.managedReferenceValue = selected switch
        {
            Affector.LifetimeLabel.instant => new InstantLifetime(),
            Affector.LifetimeLabel.limited => new LimitedLifetime(),
            Affector.LifetimeLabel.conditional => new ConditionalLifetime(),
            _ => null
        };
        
        labelProp.serializedObject.ApplyModifiedProperties();
    }

    private static void UpdateRepeatInstance(SerializedProperty labelProp, SerializedProperty repeatProp)
    {
        Affector.RepeatLabel selected = (Affector.RepeatLabel)labelProp.enumValueIndex;
        
        repeatProp.managedReferenceValue = selected switch
        {
            Affector.RepeatLabel.once => new RepeatLogic(),
            Affector.RepeatLabel.continunous => new ContinuousRepeat(),
            Affector.RepeatLabel.iterate => new IterateRepeat(),
            Affector.RepeatLabel.period => new PeriodRepeat(),
            Affector.RepeatLabel.persist => new PersistRepeat(),
            _ => null
        };
        
        labelProp.serializedObject.ApplyModifiedProperties();
    }

    private static PropertyHandler HandleLabelField(string siblingName, Action<SerializedProperty, SerializedProperty> updater)
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

    private static PropertyHandler HandleManagedReference()
    {
        return (position, property, element) =>
        {
            if (element.managedReferenceValue == null || !element.hasVisibleChildren) return 0f;

            float h = EditorGUI.GetPropertyHeight(element, true);
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, h), element, true);
            return h;
        };
    }

    private static PropertyHandler HandleFoldableProp()
    {
        return (position, property, element) =>
        {
            if(!element.hasVisibleChildren) return 0f;
            
            float h = EditorGUI.GetPropertyHeight(element, true);
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, h), element, true);
            return h;
        };
    }

    private static PropertyHandler UpdateCoordinatorGUI(string siblingName)
    {
        return (position, property, element) =>
        {
            float lineH = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, lineH), element);

            SerializedProperty sibling = property.FindPropertyRelative(siblingName);
            Affector.TargetLabel selected = (Affector.TargetLabel)element.enumValueIndex;
            
            if(selected == Affector.TargetLabel.self) TargetCoordinatorContext.SuppressCoordinatorPanel = true;
            else if(selected == Affector.TargetLabel.environment) TargetCoordinatorContext.SuppressAreaConfig = true;
            
            return lineH;
        };
    }
}
