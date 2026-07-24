using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AffecterEditorAssist
{
    public delegate void PropertyHandler(SerializedObject property, SerializedProperty element);
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
        return (property, element) =>
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(element);

            if (EditorGUI.EndChangeCheck())
            {
                SerializedProperty sibling = property.FindProperty(siblingName);
                updater(element, sibling);
            }
        };
    }

    private static PropertyHandler HandleManagedReference()
    {
        return (property, element) =>
        {
            if (element.managedReferenceValue == null || !element.hasVisibleChildren) return;

            EditorGUILayout.PropertyField(element, true);
        };
    }

    private static PropertyHandler HandleFoldableProp()
    {
        return (property, element) =>
        {
            if(!element.hasVisibleChildren) return;
            
            EditorGUILayout.PropertyField(element, true);
        };
    }

    private static PropertyHandler UpdateCoordinatorGUI(string siblingName)
    {
        return (property, element) =>
        {
            EditorGUILayout.PropertyField(element);

            SerializedProperty sibling = property.FindProperty(siblingName);
            Affecter.TargetLabel selected = (Affecter.TargetLabel)element.enumValueIndex;
            
            if(selected == Affecter.TargetLabel.self) TargetCoordinatorContext.SuppressCoordinatorPanel = true;
            else if(selected == Affecter.TargetLabel.environment) TargetCoordinatorContext.SuppressAreaConfig = true;
        };
    }
}
