using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DialogOption
{
    public string text;
    public List<DialogSequenceEntry> sequence;
}

[System.Serializable]
public struct DialogEntry
{
    public string text;
    public List<DialogOption> dialogOptions;
}

[System.Serializable]
public struct ChangeFlag
{
    public FlagContainer flagContainer;
    public string flag;
    public bool enable;
    public Optional<int> overridePriority;
}

[System.Serializable]
public class DialogSequenceEntry
{
    public Optional<DialogEntry> dialogEntry;
    public Optional<DialogAsset> dialogAsset;
    public Optional<ChangeFlag> changeFlag;
}

[CreateAssetMenu(fileName = "Dialog", menuName = "ScriptableObjects/DialogAsset", order = 1)]
public class DialogAsset : ScriptableObject
{
    public List<DialogSequenceEntry> sequence;
}
