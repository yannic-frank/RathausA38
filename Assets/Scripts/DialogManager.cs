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
            flags[flag] = priority;
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
        pairManager.SetUIInput(false);
        
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
            
        pairManager.SetUIInput(false);
        
        NextDialog();
    }

    private void NextDialog()
    {
        currentEntry = null;
        
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

            if (!DialogConditions(entry.conditions)) continue;

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

        if (entry.dialogAsset.Enabled && entry.dialogAsset.Value)
        {
            sequence.Push(new List<DialogSequenceEntry>(entry.dialogAsset.Value.sequence));
        }

        if (entry.dialogEntry.Enabled)
        {
            var dialogEntry = entry.dialogEntry.Value;
            dialogEntry.dialogOptions = new List<DialogOption>(dialogEntry.dialogOptions);

            for (int i = 0; i < dialogEntry.dialogOptions.Count; ++i)
            {
                if (!DialogConditions(dialogEntry.dialogOptions[i].conditions))
                {
                    dialogEntry.dialogOptions.RemoveAt(i);
                    --i;
                }
            }
            
            currentEntry = dialogEntry;

            bool dialogTextEmpty = currentEntry.Value.text.Length == 0;

            if (!dialogTextEmpty)
            {
                pairManager.SetUIInput(true);
            }
            
            dialogUIController.EnterDialogEntry(currentEntry.Value);

            if (dialogTextEmpty)
            {
                NextDialog();
            }
        }
        else
        {
            NextDialog();
        }
    }

    private bool DialogConditions(List<DialogCondition> conditions)
    {
        bool enable = true;
        
        foreach (var dialogCondition in conditions)
        {
            bool flagPresent = HasFlag(dialogCondition.flag);
            enable = flagPresent == dialogCondition.enabled;
            if (!enable) break;
        }

        return enable;
    }
}
