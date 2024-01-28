using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public delegate void FlagChanged(string flag, bool set, Optional<int> priority);

[System.Serializable]
public struct RandomStartupFlags
{
    public List<string> randomFlags;
}

public class DialogManager : MonoBehaviour
{
    public FlagChanged OnFlagChanged;

    public PairManager pairManager;
    public DialogUIController dialogUIController;
    public GameObject pairStart;

    public DialogAsset startupDialog;
    public List<RandomStartupFlags> startupRandomFlags = new List<RandomStartupFlags>();

    public string restartFlag = "restart";

    private Stack<List<DialogSequenceEntry>> sequence = new Stack<List<DialogSequenceEntry>>();
    private DialogEntry? currentEntry;
    private Dictionary<string, Optional<int>> flags = new Dictionary<string, Optional<int>>();
    private HashSet<string> uiInputLock = new HashSet<string>();
    
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
        Optional<int> prevPriority;
        if (flags.TryGetValue(flag, out prevPriority) == set && (!set || prevPriority.Equals(priority))) return;
        
        if (set)
        {
            flags[flag] = priority;
        }
        else
        {
            flags.Remove(flag);
        }
        
        OnFlagChanged.Invoke(flag, set, priority);
    }

    public void EnterDialog(DialogAsset dialog)
    {
        if (dialog == null) return;
        sequence.Push(new List<DialogSequenceEntry>(dialog.sequence));
        if (currentEntry == null) NextDialog();
    }

    private void Start()
    {
        if (pairManager == null) pairManager = FindObjectOfType<PairManager>();
        if (dialogUIController == null) dialogUIController = FindObjectOfType<DialogUIController>();

        dialogUIController.OnDialogCommitted += DialogUICommitted;
        dialogUIController.OnDialogOptionClicked += DialogUIOption;

        OnFlagChanged += FlagChanged;
        
        Invoke("StartGame", 1);
    }

    public void StartGame()
    {
        var oldFlags = new Dictionary<string, Optional<int>>(flags);
        foreach (var flag in oldFlags)
        {
            SetFlag(flag.Key, false, new Optional<int>());
        }
            
        if (pairStart)
        {
            GameObject current = pairManager.active;
            GameObject other = pairManager.pair2;
            if (current == pairManager.pair2)
            {
                other = pairManager.pair1;
            }

            Vector3 startPosition = pairStart.transform.position;
            current.transform.position = startPosition;
            other.transform.position = startPosition + new Vector3(1.5f,0);
        }

        foreach (RandomStartupFlags entry in startupRandomFlags)
        {
            if (entry.randomFlags.Count == 0) continue;

            string flag = entry.randomFlags[Random.Range(0, entry.randomFlags.Count)];
            SetFlag(flag, true, new Optional<int>());
        }
            
        EnterDialog(startupDialog);
        
        dialogUIController.FadeBlackIn(() => {
        }, 0);
    }

    public void RestartGame()
    {
        SetInputLock("fadeout");
        dialogUIController.FadeBlackOut(() =>
        {
            StartGame();
            UnsetInputLock("fadeout");
        });
    }

    private void FlagChanged(string flag, bool set, Optional<int> priority)
    {
        if (flag == restartFlag)
        {
            SetFlag(flag, false, new Optional<int>());
            
            RestartGame();
        }
    }

    private void DialogUIOption(int optionIndex)
    {
        UnsetInputLock("dialog");
        
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
            
        UnsetInputLock("dialog");
        
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
                SetInputLock("dialog");
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

    private void SetInputLock(string inputLock)
    {
        if (uiInputLock.Count == 0)
        {
            pairManager.SetUIInput(true);
        }
        
        uiInputLock.Add(inputLock);
    }

    private void UnsetInputLock(string inputLock)
    {
        uiInputLock.Remove(inputLock);

        if (uiInputLock.Count == 0)
        {
            pairManager.SetUIInput(false);
        }
    }
}
