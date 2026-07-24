using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AffecterDrawerAssist
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
        Affecter.LifetimeLabel selected = (Affecter.LifetimeLabel)labelProp.enumValueIndex;
        
        lifeProp.managedReferenceValue = selected switch
        {
            Affecter.LifetimeLabel.instant => new InstantLifetime(),
            Affecter.LifetimeLabel.limited => new LimitedLifetime(),
            Affecter.LifetimeLabel.conditional => new ConditionalLifetime(),
            _ => null
        };
        
        labelProp.serializedObject.ApplyModifiedProperties();
    }

    private static void UpdateRepeatInstance(SerializedProperty labelProp, SerializedProperty repeatProp)
    {
        Affecter.RepeatLabel selected = (Affecter.RepeatLabel)labelProp.enumValueIndex;
        
        repeatProp.managedReferenceValue = selected switch
        {
            Affecter.RepeatLabel.once => new RepeatLogic(),
            Affecter.RepeatLabel.continunous => new ContinuousRepeat(),
            Affecter.RepeatLabel.iterate => new IterateRepeat(),
            Affecter.RepeatLabel.period => new PeriodRepeat(),
            Affecter.RepeatLabel.persist => new PersistRepeat(),
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
            Affecter.TargetLabel selected = (Affecter.TargetLabel)element.enumValueIndex;
            
            if(selected == Affecter.TargetLabel.self) TargetCoordinatorContext.SuppressCoordinatorPanel = true;
            else if(selected == Affecter.TargetLabel.environment) TargetCoordinatorContext.SuppressAreaConfig = true;
            
            return lineH;
        };
    }
}
