using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BehaviorState))]
public class BehaviorStateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty parameters = serializedObject.FindProperty("parameters");
        SerializedProperty labelProp = serializedObject.FindProperty("effectLabel");
        SerializedProperty effectProp = serializedObject.FindProperty("affecter");

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(parameters);
        if (EditorGUI.EndChangeCheck())
        {
            UpdateEffectInstance(parameters, labelProp, effectProp);
        }
        
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(labelProp);
        if (EditorGUI.EndChangeCheck())
        {
            UpdateEffectInstance(labelProp, effectProp);
        }

        if (effectProp.managedReferenceValue != null)
        {
            EditorGUILayout.PropertyField(effectProp, true);
        };

        DrawPropertiesExcluding(serializedObject, "parameters", "effectLabel", "affecter", "m_Script");

        serializedObject.ApplyModifiedProperties();
    }

    private void UpdateEffectInstance(SerializedProperty labelProp, SerializedProperty effectProp)
    {
        PowerUp.EffectLabel selected = (PowerUp.EffectLabel)labelProp.enumValueIndex;
        Affecter affecter = effectProp.managedReferenceValue as Affecter;

        effectProp.managedReferenceValue = selected switch
        {
            PowerUp.EffectLabel.stat => new StatAffecter(affecter),
            PowerUp.EffectLabel.transform => new TransformAffecter(affecter),
            PowerUp.EffectLabel.instantiate => new InstantiationAffecter(affecter),
            PowerUp.EffectLabel.status => new StatusAffecter(affecter),
            _ => null
        };

        effectProp.serializedObject.ApplyModifiedProperties();
    }

    private void UpdateEffectInstance(SerializedProperty parameters, SerializedProperty labelProp, SerializedProperty effectProp)
    {
        EffectParameters data = parameters.objectReferenceValue as EffectParameters;
        if(data == null) return;
        
        labelProp.enumValueIndex = (int)data.EffectMode;
        PowerUp.EffectLabel selected = (PowerUp.EffectLabel)labelProp.enumValueIndex;

        effectProp.managedReferenceValue = selected switch
        {
            PowerUp.EffectLabel.stat => new StatAffecter(data),
            PowerUp.EffectLabel.transform => new TransformAffecter(data),
            PowerUp.EffectLabel.instantiate => new InstantiationAffecter(data),
            PowerUp.EffectLabel.status => new StatusAffecter(data),
            _ => null
        };

        effectProp.serializedObject.ApplyModifiedProperties();
    }
}
