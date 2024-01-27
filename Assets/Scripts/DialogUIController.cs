using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UIElements;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public delegate void DialogCommitted();
public delegate void DialogOptionClicked(int option);

public class DialogUIController : MonoBehaviour
{
    public DialogCommitted OnDialogCommitted; 
    public DialogOptionClicked OnDialogOptionClicked;

    private UIDocument uiDocument;
    private TextElement dialogText;
    private Dictionary<Button, int> optionButtons = new Dictionary<Button, int>();
    private FMOD.Studio.EVENT_CALLBACK audioCallback;
    private EventInstance audioInstance;
    private EventReference nextAudio;

    public void EnterDialogEntry(DialogEntry entry)
    {
        if (!entry.audio.IsNull)
        {
            if (audioInstance.isValid())
            {
                nextAudio = entry.audio;
                StopAudio();
            }
            else
            {
                StartAudio(entry.audio);
            }
        }
        
        if (entry.text.Length == 0) return;
        
        uiDocument.enabled = true;
        
        dialogText = uiDocument.rootVisualElement.Q<Label>("DialogText");
        dialogText.RegisterCallback<ClickEvent>(DialogTextClick);
        for (int i = 0; i < 4; ++i)
        {
            Button button = uiDocument.rootVisualElement.Q("Option" + (i + 1)) as Button;
            if (button != null)
            {
                if (i < entry.dialogOptions.Count)
                {
                    button.RegisterCallback<ClickEvent>(OptionClick);
                    optionButtons.Add(button, i);
                    button.text = entry.dialogOptions[i].text;
                }
                else
                {
                    button.parent.Remove(button);
                }
            }
        }
        
        dialogText.text = entry.text;
        }

    public void StartAudio(EventReference audioReference)
    {
        if (audioInstance.isValid())
        {
            audioInstance.setCallback(null);
            audioInstance.release();
        }
        audioInstance = FMODUnity.RuntimeManager.CreateInstance(audioReference);
        audioInstance.start();
        audioInstance.setCallback(audioCallback);
    }

    public void StopAudio()
    {
        audioInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }
    
    public void HideDialog()
    {
        foreach (var button in optionButtons)
        {
            button.Key.UnregisterCallback<ClickEvent>(OptionClick);
        }
        optionButtons.Clear();
        
        uiDocument.enabled = false;
    }
    
    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
    }

    private void OnDisable()
    {
        HideDialog();
        StopAudio();
        
        audioInstance.setCallback(null);
        audioInstance.release();
    }

    private void Start()
    {
        uiDocument.enabled = false;

        audioCallback = new EVENT_CALLBACK(AudioEventCallback);
    }

    private void OptionClick(ClickEvent evt)
    {
        if (!uiDocument.enabled) return;
        
        Button button = evt.currentTarget as Button;
        int option;
        if (button != null && optionButtons.TryGetValue(button, out option))
        {
            CommitOption(option);
        }
    }

    private void DialogTextClick(ClickEvent evt)
    {
        OnDialogCommit();
    }

    public void CommitOption(int option)
    {
        if (!uiDocument.enabled || optionButtons.Count == 0) return;
        HideDialog();
        OnDialogOptionClicked.Invoke(option);
    }

    public void OnDialogCommit()
    {
        if (!uiDocument.enabled || optionButtons.Count > 0) return;
        HideDialog();
        OnDialogCommitted.Invoke();
    }
    
    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    private FMOD.RESULT AudioEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr p1, IntPtr p2)
    {
        if (type == EVENT_CALLBACK_TYPE.STOPPED)
        {
            StopAudio();
            if (!nextAudio.IsNull)
            {
                StartAudio(nextAudio);
            }
        }
        return FMOD.RESULT.OK;
    }
}