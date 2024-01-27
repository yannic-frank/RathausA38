using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public delegate void DialogCommitted();
public delegate void DialogOptionClicked(int option);

public class DialogUIController : MonoBehaviour
{
    public DialogCommitted OnDialogCommitted; 
    public DialogOptionClicked OnDialogOptionClicked;

    private UIDocument uiDocument;
    private TextElement dialogText;
    private Dictionary<Button, int> optionButtons = new Dictionary<Button, int>();

    public void ShowDialog(DialogEntry entry)
    {
        uiDocument.enabled = true;
        
        dialogText = uiDocument.rootVisualElement.Q<Label>("DialogText");
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
    }

    private void Start()
    {
        uiDocument.enabled = false;
    }

    private void OptionClick(ClickEvent evt)
    {
        if (!uiDocument.enabled) return;
        
        Button button = evt.currentTarget as Button;
        int option;
        if (button != null && optionButtons.TryGetValue(button, out option))
        {
            HideDialog();
            OnDialogOptionClicked.Invoke(option);
        }
    }

    public void OnDialogCommit()
    {
        if (!uiDocument.enabled && optionButtons.Count > 0) return;
        HideDialog();
        OnDialogCommitted.Invoke();
    }
}