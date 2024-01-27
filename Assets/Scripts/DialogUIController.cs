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
    private Label dialogText;
    private Dictionary<Button, int> optionButtons = new Dictionary<Button, int>();

    public void ShowDialog(DialogEntry entry)
    {
        uiDocument.enabled = true;
        dialogText.text = entry.text;
    }
    
    public void HideDialog()
    {
        uiDocument.enabled = false;
    }
    
    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();

        dialogText = uiDocument.rootVisualElement.Q("DialogText") as Label;
        for (int i = 0; i < 4; ++i)
        {
            Button button = uiDocument.rootVisualElement.Q("Option" + (i + 1)) as Button;
            if (button != null)
            {
                button.RegisterCallback<ClickEvent>(OptionClick);
                optionButtons.Add(button, i);
            }
        }
    }

    private void OnDisable()
    {
        foreach (var button in optionButtons)
        {
            button.Key.UnregisterCallback<ClickEvent>(OptionClick);
        }
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
        if (!uiDocument.enabled) return;
        HideDialog();
        OnDialogCommitted.Invoke();
    }
}