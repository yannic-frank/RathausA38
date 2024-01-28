using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public delegate void DialogCommitted();
public delegate void DialogOptionClicked(int option);

public class DialogUIController : MonoBehaviour
{
    public DialogCommitted OnDialogCommitted; 
    public DialogOptionClicked OnDialogOptionClicked;

    public float blendLength = 5;
    public float textSpeed = 50;

    private UIDocument uiDocument;
    private Label dialogText;
    private Dictionary<Button, int> optionButtons = new Dictionary<Button, int>();
    private VisualElement fadeBlack;
    private VisualElement dialogBox;
    private VisualElement dialogEntity;
    private Label dialogEntityName;
    private VisualElement dialogEntityImage;

    private int dialogOptions = 0;
    private string dialogTargetText = "";
    private DateTime dialogStart;
    
    private FMOD.Studio.EVENT_CALLBACK audioCallback;
    private EventInstance audioInstance;
    private EventReference nextAudio;
    
    private Action fadeBlackCallback;

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

        dialogOptions = entry.dialogOptions.Count;
        foreach (var button in optionButtons)
        {
            if (button.Value < dialogOptions)
            {
                button.Key.text = entry.dialogOptions[button.Value].text;
                button.Key.visible = true;
            }
            else
            {
                button.Key.visible = false;
            }
        }

        dialogTargetText = entry.text;
        dialogText.text = "";
        dialogStart = DateTime.Now;
        dialogBox.visible = true;
        if (entry.entity != null)
        {
            dialogEntity.style.display = DisplayStyle.Flex;
            dialogEntityName.text = entry.entity.entityName;
            dialogEntityImage.style.backgroundImage = new StyleBackground(entry.entity.entityImage);
        }
        else
        {
            dialogEntity.style.display = DisplayStyle.None;
        }
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
        dialogBox.visible = false;
        
        foreach (var button in optionButtons)
        {
            button.Key.visible = false;
        }
    }
    
    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        
        dialogText = uiDocument.rootVisualElement.Q<Label>("DialogText");
        
        dialogBox = uiDocument.rootVisualElement.Q("DialogBox");
        dialogBox.RegisterCallback<ClickEvent>(DialogBoxClick);
        dialogBox.visible = false;

        dialogEntity = uiDocument.rootVisualElement.Q("DialogEntity");
        dialogEntityName = uiDocument.rootVisualElement.Q<Label>("DialogEntityName");
        dialogEntityImage = uiDocument.rootVisualElement.Q("DialogEntityImage");

        optionButtons.Clear();
        fadeBlack = uiDocument.rootVisualElement.Q("BlackFade");
        fadeBlack.RemoveFromClassList("hidden");
        fadeBlack.RegisterCallback<TransitionEndEvent>(FadeBlackCallback);
        
        for (int i = 0; i < 4; ++i)
        {
            Button button = uiDocument.rootVisualElement.Q("Option" + (i + 1)) as Button;
            if (button != null)
            {
                optionButtons.Add(button, i);
                button.RegisterCallback<ClickEvent>(OptionClick);
            }
        }
    }

    private void OnDisable()
    {
        HideDialog();
        StopAudio();
        
        foreach (var button in optionButtons)
        {
            button.Key.UnregisterCallback<ClickEvent>(OptionClick);
        }
        
        audioInstance.setCallback(null);
        audioInstance.release();
    }

    private void Start()
    {
        audioCallback = new EVENT_CALLBACK(AudioEventCallback);
    }

    private void Update()
    {
        int index = Math.Clamp((int)(((DateTime.Now - dialogStart)).TotalSeconds * textSpeed), 0, dialogTargetText.Length);
        dialogText.text = dialogTargetText.Substring(0, index);
    }

    private void OptionClick(ClickEvent evt)
    {
        if (!dialogBox.visible) return;
        
        Button button = evt.currentTarget as Button;
        int option;
        if (button != null && optionButtons.TryGetValue(button, out option))
        {
            if (option >= dialogOptions) return;
            CommitOption(option);
        }
    }

    private void DialogBoxClick(ClickEvent evt)
    {
        OnDialogCommit();
    }

    public void CommitOption(int option)
    {
        if (!dialogBox.visible || dialogOptions == 0) return;
        HideDialog();
        OnDialogOptionClicked.Invoke(option);
    }

    public void OnDialogCommit()
    {
        if (!dialogBox.visible || dialogOptions > 0) return;
        HideDialog();
        if (OnDialogCommitted != null) OnDialogCommitted.Invoke();
    }

    public void FadeBlackIn(Action callback, float timeOffset = 0)
    {
        fadeBlackCallback = callback;

        fadeBlack.AddToClassList("hidden");
    }
    
    public void FadeBlackOut(Action callback)
    {
        fadeBlackCallback = callback;

        fadeBlack.RemoveFromClassList("hidden");
    }

    private void FadeBlackCallback(TransitionEndEvent evt)
    {
        if (fadeBlackCallback != null)
        {
            fadeBlackCallback.Invoke();
            fadeBlackCallback = null;
        }
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