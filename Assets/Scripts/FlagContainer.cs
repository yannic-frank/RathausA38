using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;

public delegate void FlagChanged(string flag, bool set, Optional<int> priority);

public class FlagContainer : MonoBehaviour
{
    public FlagChanged OnFlagChanged;
    
    private Dictionary<string, Optional<int>> flags;

    public bool HasFlag(string flag)
    {
        return flags.ContainsKey(flag);
    }

    public bool GetFlag(string flag, out Optional<int> priority)
    {
        return flags.TryGetValue(flag, out priority);
    }

    public void SetFlag(string flag, bool set, Optional<int> priority)
    {
        if (set)
        {
            flags.Add(flag, priority);
        }
        else
        {
            flags.Remove(flag);
        }
    }
}
