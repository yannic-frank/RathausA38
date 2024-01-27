using UnityEngine;
using UnityEngine.InputSystem;

public class PairManager : MonoBehaviour
{
    public GameObject pair1;
    public GameObject pair2;

    GameObject active;

    // Start is called before the first frame update
    void Start()
    {
        active = pair1;
        pair1.GetComponent<PartnerMovement>().partner = pair2;
        pair2.GetComponent<PartnerMovement>().partner = pair1;

        ActivatePair(pair1);
        DeactivatePair(pair2);
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle active pair with Tab key
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleActive();
        }
    }

    void ToggleActive()
    {
        // Swap the active pair
        active = (active == pair1) ? pair2 : pair1;

        // Activate and deactivate scripts based on the active pair
        ActivatePair(active);
        DeactivatePair((active == pair1) ? pair2 : pair1);

        // Set the camera target to the active pair
        SetCameraTarget(active.transform);
    }

    void ActivatePair(GameObject pair)
    {
        if (pair != null)
        {
            // Activate PlayerMovement script
            PlayerMovement playerMovement = pair.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
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
}
