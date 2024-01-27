using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PartnerMovement : MonoBehaviour
{

    public float distanceToPartner = 1.5f;
    public float movementSpeed = 10.0f;
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

            // Calculate the direction from the current position to the target position
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

            // Calculate the new position with the desired distance
            Vector2 newPosition = targetPosition - direction * distanceToPartner;

            // Set the position directly without smoothing
            physics.position = newPosition;
        }
    }
}
