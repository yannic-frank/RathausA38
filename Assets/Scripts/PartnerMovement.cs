using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PartnerMovement : MonoBehaviour
{

    public float distanceToPartner = 1.5f;
    public float movementSpeed = 1.0f;
    public Rigidbody2D physics;

    public GameObject partner;

    // Start is called before the first frame update
    void Start()
    {
        physics= GetComponent<Rigidbody2D>();    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        FollowPartner();
    }

    void FollowPartner()
    {
        if (partner != null)
        {
            Vector2 targetPosition = partner.transform.position;
            Vector2 currentPosition = transform.position;

            // Calculate the direction from the current position to the target position
            Vector2 direction = (targetPosition - currentPosition).normalized;

            // Calculate the new position with the desired distance
            Vector2 newPosition = targetPosition - direction * distanceToPartner;

            // Apply damping to smooth out the movement
            float damping = 0.2f; // Adjust this value to control the speed of the movement
            Vector2 smoothedPosition = Vector2.Lerp(currentPosition, newPosition, damping);

            // Move towards the smoothed position with a speed close to player movement speed
            physics.velocity = (smoothedPosition - currentPosition).normalized * 1.0f;
        }
    }
}
