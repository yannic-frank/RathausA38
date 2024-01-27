using System;
using UnityEngine;

public class Example : MonoBehaviour
{
    public DialogAsset dialog;
    public DialogManager dialogManager;

    private void Start()
    {
        if (dialogManager == null) dialogManager = FindObjectOfType<DialogManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        dialogManager.EnterDialog(dialog);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        dialogManager.EnterDialog(dialog);
    }
}
