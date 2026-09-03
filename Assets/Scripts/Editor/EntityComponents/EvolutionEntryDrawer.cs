using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EvolutionEntry))]
public class EvolutionEntryDrawer : PropertyDrawer
{
    const float Padding = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float y = position.y;

        var thresholdProp = property.FindPropertyRelative("threshold");
        var effectProp    = property.FindPropertyRelative("effect");
        var payloadProp   = property.FindPropertyRelative("payload");

        // Threshold
        Rect thresholdRect = new(
            position.x,
            y,
            position.width,
            EditorGUIUtility.singleLineHeight
        );
        EditorGUI.PropertyField(thresholdRect, thresholdProp);
        y += EditorGUIUtility.singleLineHeight + Padding;

        // Effect
        Rect effectRect = new(
            position.x,
            y,
            position.width,
            EditorGUIUtility.singleLineHeight
        );
        EditorGUI.PropertyField(effectRect, effectProp);
        y += EditorGUIUtility.singleLineHeight + Padding;

        // Stop if no effect selected
        if (effectProp.objectReferenceValue == null)
        {
            EditorGUI.EndProperty();
            return;
        }

        var effect = effectProp.objectReferenceValue as EvolutionEffect;
        var requiredPayloadType = effect.PayloadType;

        // Ensure correct payload instance exists
        if (payloadProp.managedReferenceValue == null ||
            payloadProp.managedReferenceValue.GetType() != requiredPayloadType)
        {
            payloadProp.managedReferenceValue =
                System.Activator.CreateInstance(requiredPayloadType);
        }

        // Payload
        float payloadHeight = EditorGUI.GetPropertyHeight(payloadProp, true);
        Rect payloadRect = new (
            position.x,
            y,
            position.width,
            payloadHeight
        );
        EditorGUI.PropertyField(payloadRect, payloadProp, true);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;

        height += EditorGUIUtility.singleLineHeight + Padding; // threshold
        height += EditorGUIUtility.singleLineHeight + Padding; // effect

        var payloadProp = property.FindPropertyRelative("payload");
        if (payloadProp != null && payloadProp.managedReferenceValue != null)
        {
            height += EditorGUI.GetPropertyHeight(payloadProp, true);
        }

        return height;
    }
}