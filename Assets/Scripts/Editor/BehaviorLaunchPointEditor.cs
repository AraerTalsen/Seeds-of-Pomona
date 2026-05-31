using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BehaviorLaunchPoint))]
public class BehaviorLaunchPointEditor : Editor
{
    private string _directory = "Assets/Resources/ScriptableObjects/BehaviorTrees/";
    private string _newFolderName = "";
    private Dictionary<int, List<IBehaviorContext.WeightedState>> _stateCache = new();
    
    public override void OnInspectorGUI()
    {
        SerializedProperty nodeProp = serializedObject.FindProperty("behaviorBase");

        BehaviorState baseNode = (BehaviorState)nodeProp.objectReferenceValue;
        _newFolderName = EditorGUILayout.TextField("Override Folder", _newFolderName);

        if (GUILayout.Button("Create Unique Override"))
        {
            BehaviorState cloned = BehaviorLaunchPoint.DeepCloneNode(baseNode, BuildAsset);
            nodeProp.objectReferenceValue = cloned;
            serializedObject.ApplyModifiedProperties();
        }
        
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(nodeProp);
        if (EditorGUI.EndChangeCheck())
        {
            baseNode = (BehaviorState)nodeProp.objectReferenceValue;
            nodeProp.objectReferenceValue = baseNode;
            serializedObject.ApplyModifiedProperties();
        }

        DrawNodeTree((BehaviorState)nodeProp.objectReferenceValue);
    }

    private void DrawNodeTree(BehaviorState state)
    {
        if(state == null || state is not BehaviorContext) return;

        BehaviorContext context = (BehaviorContext)state;

        SerializedObject so = new(state);
        SerializedProperty posStatesProp = so.FindProperty("possibleStates");

        List<IBehaviorContext.WeightedState> cachedValue = RetrieveCachedValue(context, posStatesProp, out int id);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField
        (
            posStatesProp, new GUIContent($"{context.name} {(_newFolderName.CompareTo("") == 0 ? "(Template)" : $"({_newFolderName})")} States")
        );
        if(EditorGUI.EndChangeCheck())
        {
            _stateCache[id] = context.PossibleStates = ApplyEndChange(so, cachedValue);
            so.ApplyModifiedProperties();
        }

        EditorGUI.indentLevel++;
        foreach ((BehaviorState s, _) in context.PossibleStates)
            DrawNodeTree(s);
        EditorGUI.indentLevel--;
    }

    private List<IBehaviorContext.WeightedState> RetrieveCachedValue(BehaviorContext state, SerializedProperty posStatesProp, out int id)
    {
        id = state.GetInstanceID();
        if(!_stateCache.ContainsKey(id))
        {
            _stateCache[id] = WeightedStateListValue(posStatesProp);
        }
        
        return _stateCache[id];
    }

    private List<IBehaviorContext.WeightedState> ApplyEndChange(SerializedObject node, List<IBehaviorContext.WeightedState> cachedValue)
    {
        SerializedProperty statesUpdate = node.FindProperty("possibleStates");
        List<IBehaviorContext.WeightedState> updatedValue = WeightedStateListValue(statesUpdate);
        if(updatedValue.Count >= cachedValue.Count)
        {
            for(int i = 0; i < updatedValue.Count; i++)
            {
                BehaviorState element = updatedValue[i].State;

                bool outOfBounds = i > cachedValue.Count - 1;
                if(outOfBounds)
                {
                    updatedValue[i] = new (BehaviorLaunchPoint.DeepCloneNode(element, BuildAsset), updatedValue[i].Weight);
                    break;
                }

                bool isMismatched = element == null || element.name != cachedValue[i].State.name;
                if(isMismatched)
                {
                    updatedValue[i] = new (BehaviorLaunchPoint.DeepCloneNode(element, BuildAsset), updatedValue[i].Weight);
                    break;
                }
            }
            return updatedValue;
        }

        return cachedValue;
    }

    private List<IBehaviorContext.WeightedState> WeightedStateListValue(SerializedProperty listProp)
    {
        List<IBehaviorContext.WeightedState> newList = new();
        for (int i = 0; i < listProp.arraySize; i++)
        {
            SerializedProperty element = listProp.GetArrayElementAtIndex(i);
            SerializedProperty stateProp = element.FindPropertyRelative("_state");
            SerializedProperty weightProp = element.FindPropertyRelative("_weight");

            BehaviorState state = stateProp.objectReferenceValue != null ? stateProp.objectReferenceValue as BehaviorState : null;
            int weight = weightProp.intValue;
            newList.Add(new (state, weight));
        }
        return newList;
    }

    private void PrintNode(BehaviorState state)
    {
        if(state == null) return;

        Debug.Log(state.name);

        if(state is BehaviorContext context)
        {
            foreach((BehaviorState s, _) in context.PossibleStates)
            {
                PrintNode(s);
            }
        }
    }

    private void BuildAsset(ScriptableObject asset)
    {
        string folderPath = _directory + _newFolderName;

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder(_directory.TrimEnd('/'), _newFolderName);
        }
        
        string path = $"{folderPath}/{asset.name}.asset";
        path = AssetDatabase.GenerateUniqueAssetPath(path);
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
    }
}
