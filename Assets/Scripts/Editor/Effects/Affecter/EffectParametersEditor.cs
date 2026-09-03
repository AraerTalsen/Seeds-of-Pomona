using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(EffectParameters))]
public class EffectParametersEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        AffecterEditorAssist.InitHandlers();
        TargetCoordinatorContext.Reset();

        SerializedProperty iter = serializedObject.GetIterator();
        iter.NextVisible(true);

        while(iter.NextVisible(false))
        {
            if (AffecterEditorAssist.handlerMap.TryGetValue(iter.name, out AffecterEditorAssist.PropertyHandler handler))
            {
                handler(serializedObject, iter.Copy());
            }
            else
            {
                EditorGUILayout.PropertyField(iter, true);
            }
        }

        serializedObject.ApplyModifiedProperties();
    } 
}
