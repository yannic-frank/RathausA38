using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct DialogOption
{
    public List<DialogCondition> conditions;
    public string text;
    public List<DialogSequenceEntry> sequence;
}

[System.Serializable]
public struct DialogEntry
{
    public DialogEntity entity;
    public string text;
    public FMODUnity.EventReference audio;
    public List<DialogOption> dialogOptions;
}

[System.Serializable]
public struct ChangeFlag
{
    [DoNotSerialize]
    public string flag;
    public bool enable;
    public Optional<int> overridePriority;
}

[System.Serializable]
public struct DialogCondition
{
    public string flag;
    public bool enabled;
}

[System.Serializable]
public class DialogSequenceEntry
{
    public List<DialogCondition> conditions;
    public Optional<ChangeFlag> changeFlag;
    public Optional<DialogEntry> dialogEntry;
    public Optional<DialogAsset> dialogAsset;
}

[CreateAssetMenu(fileName = "Dialog", menuName = "ScriptableObjects/DialogAsset", order = 1)]
public class DialogAsset : ScriptableObject
{
    public List<DialogSequenceEntry> sequence;
}

namespace Editor
{
    using UnityEditor;
    using UnityEngine;
    
    [CustomPropertyDrawer(typeof(DialogCondition))]
    public class DialogSequenceEntryPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var flagProperty = property.FindPropertyRelative("flag");
            return EditorGUI.GetPropertyHeight(flagProperty);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var flagProperty = property.FindPropertyRelative("flag");
            var enabledProperty = property.FindPropertyRelative("enabled");

            EditorGUI.BeginProperty(position, label, property);
            position.width -= 24;
            EditorGUI.PropertyField(position, flagProperty, GUIContent.none, true);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            position.x += position.width + 24;
            position.width = position.height = EditorGUI.GetPropertyHeight(enabledProperty);
            position.x -= position.width;
            EditorGUI.PropertyField(position, enabledProperty, GUIContent.none);
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
    
    [CustomPropertyDrawer(typeof(ChangeFlag))]
    public class ChangeFlagPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var flagProperty = property.FindPropertyRelative("flag");
            var enableProperty = property.FindPropertyRelative("enable");
            var priorityProperty = property.FindPropertyRelative("overridePriority");
            float height = EditorGUI.GetPropertyHeight(flagProperty);
            if (enableProperty.boolValue) height += EditorGUI.GetPropertyHeight(priorityProperty);
            return height;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var flagProperty = property.FindPropertyRelative("flag");
            var enableProperty = property.FindPropertyRelative("enable");
            var priorityProperty = property.FindPropertyRelative("overridePriority");

            Rect originalPosition = position;

            EditorGUI.BeginProperty(position, label, property);
            position.width -= 24;
            EditorGUI.PropertyField(position, flagProperty, label, true);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            position.x += position.width + 24;
            position.width = position.height = EditorGUI.GetPropertyHeight(enableProperty);
            position.x -= position.width;
            EditorGUI.PropertyField(position, enableProperty, GUIContent.none);
            EditorGUI.indentLevel = indent;

            if (enableProperty.boolValue)
            {
                position = originalPosition;
                position.y = EditorGUI.GetPropertyHeight(flagProperty);
                position.x += 24;
                position.width -= 24;
                EditorGUI.PropertyField(position, priorityProperty, new GUIContent(priorityProperty.displayName));
            }
            
            EditorGUI.EndProperty();
        }
    }
}
