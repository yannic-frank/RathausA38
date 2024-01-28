using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PairManager : MonoBehaviour
{
    public PlayerInput playerInput;
    public DialogUIController uiController;
    public DialogManager dialogManager;
    
    public GameObject pair1;
    public GameObject pair2;

    public GameObject active;
    private PlayerMovement activeMovement;

    public float distanceTrigger = 10.0f;
    public DialogAsset distanceTriggerDialog;
    public DateTime distanceTriggerLast = DateTime.Now;
    public string distanceTriggerFlag = "distanceTrigger";

    private bool movementEnabled = true;

    void Start()
    {
        if (playerInput == null) playerInput = FindObjectOfType<PlayerInput>();
        if (uiController == null) uiController = FindObjectOfType<DialogUIController>();
        if (dialogManager == null) dialogManager = FindObjectOfType<DialogManager>();
        
        if (active == null) active = pair1;
        
        SetCameraTarget(pair1.transform);
        pair1.GetComponent<PartnerMovement>().partner = pair2;
        pair2.GetComponent<PartnerMovement>().partner = pair1;

        ActivatePair(pair1);
        DeactivatePair(pair2);
    }

    void Update()
    {
        if ((pair1.transform.position - pair2.transform.position).magnitude > distanceTrigger)
        {
            if ((DateTime.Now - distanceTriggerLast).Duration() > TimeSpan.FromSeconds(5))
            {
                distanceTriggerLast = DateTime.Now;

                if (!dialogManager.HasFlag(distanceTriggerFlag))
                {
                    dialogManager.SetFlag(distanceTriggerFlag, true, new Optional<int>());
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    dialogManager.EnterDialog(distanceTriggerDialog);
                }
            }
        }
    }

    void OnToggleActive()
    {
        if (!movementEnabled) return;
        
        // Swap the active pair
        active = (active == pair1) ? pair2 : pair1;

        // Activate and deactivate scripts based on the active pair
        ActivatePair(active);
        DeactivatePair((active == pair1) ? pair2 : pair1);

        // Set the camera target to the active pair
        SetCameraTarget(active.transform);
    }

    public void OnSubmit()
    {
        uiController.OnDialogCommit();
    }

    public void OnOption1()
    {
        uiController.CommitOption(0);
    }
    
    public void OnOption2()
    {
        uiController.CommitOption(1);
    }
    
    public void OnOption3()
    {
        uiController.CommitOption(2);
    }
    
    public void OnOption4()
    {
        uiController.CommitOption(4);
    }
    
    public void OnMove(InputValue value)
    {
        if (activeMovement)
        {
            activeMovement.OnMove(value);
        }
    }

    public void OnExit()
    {
        Application.Quit();
    }

    void ActivatePair(GameObject pair)
    {
        if (pair != null)
        {
            // Activate PlayerMovement script
            activeMovement = pair.GetComponent<PlayerMovement>();
            if (activeMovement != null)
            {
                activeMovement.enabled = true;
            }

            // Deactivate PartnerMovement script
            PartnerMovement partnerMovement = pair.GetComponent<PartnerMovement>();
            if (partnerMovement != null)
            {
                partnerMovement.enabled = false;
            }
        }
    }

    void DeactivatePair(GameObject pair)
    {
        if (pair != null)
        {
            // Deactivate PlayerMovement script
            PlayerMovement playerMovement = pair.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }

            // Activate PartnerMovement script
            PartnerMovement partnerMovement = pair.GetComponent<PartnerMovement>();
            if (partnerMovement != null)
            {
                partnerMovement.enabled = true;
            }
        }
    }

    void SetCameraTarget(Transform target)
    {
        // Get the MainCamera from the scene
        GameObject mainCamera = GameObject.FindWithTag("MainCamera");

        if (mainCamera != null && mainCamera.TryGetComponent<SmoothCameraFollow>(out var smoothFollowScript))
        {
            smoothFollowScript.target = target;
        }
    }

    public void SetUIInput(bool enable)
    {
        movementEnabled = !enable;
        active.GetComponent<PlayerMovement>().enabled = !enable;
        if (enable)
        {
            playerInput.SwitchCurrentActionMap("UI");
        }
        else
        {
            playerInput.SwitchCurrentActionMap("Player");
        }
    }
}
