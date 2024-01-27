using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void FlagChanged(string flag, bool set, Optional<int> priority);

public class DialogManager : MonoBehaviour
{
    private Stack<List<DialogSequenceEntry>> sequence = new Stack<List<DialogSequenceEntry>>();
    private DialogEntry? currentEntry;
    
    public FlagChanged OnFlagChanged;
    
    private Dictionary<string, Optional<int>> flags = new Dictionary<string, Optional<int>>();

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

    public void EnterDialog(DialogAsset dialog)
    {
        sequence.Push(dialog.sequence);
        if (currentEntry == null) NextDialog();
    }
    
    public void DialogResponse(int optionIndex)
    {
        if (currentEntry.HasValue)
        {
            DialogOption option = currentEntry.Value.dialogOptions[optionIndex];
            sequence.Push(new List<DialogSequenceEntry>(option.sequence));
            NextDialog();
        }
    }

    public void NextDialog()
    {
        List<DialogSequenceEntry> entries;
        while (sequence.TryPeek(out entries))
        {
            if (entries.Count < 1) continue;
            
            DialogSequenceEntry entry = entries[0];
            entries.RemoveAt(0);

            HandleSequenceEntry(entry);
            
            break;
        }
    }

    private void HandleSequenceEntry(DialogSequenceEntry entry)
    {
        if (entry.changeFlag.Enabled)
        {
            ChangeFlag opt = entry.changeFlag.Value;
            SetFlag(opt.flag, opt.enable, opt.overridePriority);
        }

        if (entry.dialogAsset.Enabled)
        {
            sequence.Push(new List<DialogSequenceEntry>(entry.dialogAsset.Value.sequence));
        }

        if (entry.dialogEntry.Enabled)
        {
            currentEntry = entry.dialogEntry.Value;
            
            // Show Current Entry
        }
        else
        {
            NextDialog();
        }
    }
}
