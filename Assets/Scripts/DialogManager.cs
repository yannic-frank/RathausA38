using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void FlagChanged(string flag, bool set, Optional<int> priority);

public class DialogManager : MonoBehaviour
{
    public FlagChanged OnFlagChanged;

    public PairManager pairManager;
    public DialogUIController dialogUIController;
    
    private Stack<List<DialogSequenceEntry>> sequence = new Stack<List<DialogSequenceEntry>>();
    private DialogEntry? currentEntry;
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
        sequence.Push(new List<DialogSequenceEntry>(dialog.sequence));
        if (currentEntry == null) NextDialog();
    }

    private void Start()
    {
        if (pairManager == null) pairManager = FindObjectOfType<PairManager>();
        if (dialogUIController == null) dialogUIController = FindObjectOfType<DialogUIController>();

        dialogUIController.OnDialogCommitted += DialogUICommitted;
        dialogUIController.OnDialogOptionClicked += DialogUIOption;
    }

    private void DialogUIOption(int optionIndex)
    {
        pairManager.SetInputEnabled(true);
        
        if (currentEntry.HasValue && optionIndex < currentEntry.Value.dialogOptions.Count)
        {
            DialogOption option = currentEntry.Value.dialogOptions[optionIndex];
            sequence.Push(new List<DialogSequenceEntry>(option.sequence));
            NextDialog();
        }
    }

    private void DialogUICommitted()
    {
        if (!currentEntry.HasValue || currentEntry.Value.dialogOptions.Count > 0) return;
            
        pairManager.SetInputEnabled(true);
        
        NextDialog();
    }

    private void NextDialog()
    {
        List<DialogSequenceEntry> entries;
        while (sequence.TryPeek(out entries))
        {
            if (entries.Count < 1)
            {
                sequence.Pop();
                continue;
            }
            
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

            pairManager.SetInputEnabled(false);
            dialogUIController.ShowDialog(currentEntry.Value);
        }
        else
        {
            NextDialog();
        }
    }
}
