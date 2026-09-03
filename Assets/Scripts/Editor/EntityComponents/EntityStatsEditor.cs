using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(EntityStats))]
[CanEditMultipleObjects]
public class EntityStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawPropertiesExcluding(serializedObject, "stats", "m_Script");

        DrawStatBlock();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawStatBlock()
    {
        SerializedProperty statsProp = serializedObject.FindProperty("stats");
        SerializedProperty baseStatsArrayProp = statsProp.FindPropertyRelative("baseStats");
        int enumCount = Enum.GetValues(typeof(Stats)).Length;

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
    }
}
