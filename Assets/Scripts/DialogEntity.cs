using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogEntity", menuName = "ScriptableObjects/DialogEntity", order = 1)]
public class DialogEntity : ScriptableObject
{
    public string entityName;
    public Texture2D entityImage;
}
