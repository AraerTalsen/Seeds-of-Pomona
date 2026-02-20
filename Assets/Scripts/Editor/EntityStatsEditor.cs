using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(EntityStats))]
[CanEditMultipleObjects]
public class EntityStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (target == null)
            return;

        serializedObject.Update();

        SerializedProperty statsProp = serializedObject.FindProperty("stats");

        if (statsProp == null)
        {
            EditorGUILayout.HelpBox("Could not find 'stats' property.", MessageType.Error);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        SerializedProperty baseStatsArrayProp = statsProp.FindPropertyRelative("baseStats");

        if (baseStatsArrayProp == null)
        {
            EditorGUILayout.HelpBox("Stat array missing.", MessageType.Error);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        int enumCount = Enum.GetValues(typeof(Stats)).Length;

        if (baseStatsArrayProp.arraySize != enumCount)
            baseStatsArrayProp.arraySize = enumCount;

        EditorGUILayout.LabelField("Base Stats", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        for (int i = 0; i < enumCount; i++)
        {
            SerializedProperty element = baseStatsArrayProp.GetArrayElementAtIndex(i);

            EditorGUILayout.PropertyField(
                element,
                new GUIContent(((Stats)i).ToString())
            );
        }

        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}
