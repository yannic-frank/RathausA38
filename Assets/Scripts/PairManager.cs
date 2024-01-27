using UnityEngine;
using UnityEngine.InputSystem;

public class PairManager : MonoBehaviour
{
    public GameObject pair1;
    public GameObject pair2;

    public GameObject active;
    private PlayerMovement activeMovement;

    public PlayerInput playerInput;
    public DialogUIController uiController;

    private bool movementEnabled = true;

    void Start()
    {
        if (playerInput == null) playerInput = FindObjectOfType<PlayerInput>();
        if (uiController == null) uiController = FindObjectOfType<DialogUIController>();
        
        active = pair1;
        SetCameraTarget(pair1.transform);
        pair1.GetComponent<PartnerMovement>().partner = pair2;
        pair2.GetComponent<PartnerMovement>().partner = pair1;

        ActivatePair(pair1);
        DeactivatePair(pair2);
    }

    void Update()
    {
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
