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
    public string text;
    public DialogEntity entity;
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
    public Optional<DialogEntry> dialogEntry;
    public Optional<DialogAsset> dialogAsset;
    public Optional<ChangeFlag> changeFlag;
}

[CreateAssetMenu(fileName = "Dialog", menuName = "ScriptableObjects/DialogAsset", order = 1)]
public class DialogAsset : ScriptableObject
{
    public List<DialogSequenceEntry> sequence;
}
