using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PUp))]
public class PUpEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty labelProp = serializedObject.FindProperty("effectLabel");
        SerializedProperty effectProp = serializedObject.FindProperty("affector");

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

        DrawPropertiesExcluding(serializedObject, "effectLabel", "affector", "m_Script");

        serializedObject.ApplyModifiedProperties();
    }

    private void UpdateEffectInstance(SerializedProperty labelProp, SerializedProperty effectProp)
    {
        PUp.EffectLabel selected = (PUp.EffectLabel)labelProp.enumValueIndex;

        effectProp.managedReferenceValue = selected switch
        {
            PUp.EffectLabel.stat => new StatAffector(effectProp.managedReferenceValue as Affector),
            PUp.EffectLabel.transform => new TransformAffector(effectProp.managedReferenceValue as Affector),
            PUp.EffectLabel.instantiate => new InstantiationAffector(effectProp.managedReferenceValue as Affector),
            PUp.EffectLabel.status => new StatusAffector(effectProp.managedReferenceValue as Affector),
            _ => null
        };

        effectProp.serializedObject.ApplyModifiedProperties();
    }
}
