using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public Rigidbody2D physics;
    public InputAction playerControls;

    private PartnerMovement partnerMovement;


    private Vector2 moveDirection = Vector2.zero;

    void Start()
    {
        partnerMovement= GetComponent<PartnerMovement>();
        // Enable player controls
        playerControls.Enable();
    }

    private void FixedUpdate()
    {
       Move();
    }

    void OnMove(InputValue value)
    {
        moveDirection = value.Get<Vector2>();
    }

    void Move()
    {
        physics.velocity = moveDirection * movementSpeed;
    }

    

    void Update()
    {
      
    }
}
