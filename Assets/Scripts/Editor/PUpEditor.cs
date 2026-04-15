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
        SerializedProperty effectProp = serializedObject.FindProperty("effect");

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(labelProp);
        if (EditorGUI.EndChangeCheck())
        {
            UpdateEffectInstance(labelProp, effectProp);
        }

        if (effectProp.managedReferenceValue != null)
        {
            EditorGUILayout.PropertyField(effectProp, true);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void UpdateEffectInstance(SerializedProperty labelProp, SerializedProperty effectProp)
    {
        PUp.EffectLabel selected = (PUp.EffectLabel)labelProp.enumValueIndex;

        effectProp.managedReferenceValue = selected switch
        {
            PUp.EffectLabel.transform => new Effect(effectProp.managedReferenceValue as Effect),
            PUp.EffectLabel.stat => new Stat(effectProp.managedReferenceValue as Effect),
            _ => null
        };

        effectProp.serializedObject.ApplyModifiedProperties();
    }
}
