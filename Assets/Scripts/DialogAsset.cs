using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog", menuName = "ScriptableObjects/DialogAsset", order = 1)]
public class DialogAsset : ScriptableObject
{
    public string text;
    public List<DialogAsset> nextDialog;
}
