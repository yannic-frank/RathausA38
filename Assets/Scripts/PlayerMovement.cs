using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public Rigidbody2D physics;
    public InputAction playerControls;

    private PlayerInput playerInput;

    private Vector2 moveDirection = Vector2.zero;

    void Start()
    {
        // Get the PlayerInput component from the parent GameObject
        playerInput = GetComponentInParent<PlayerInput>();

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
        if (playerInput)
        {
            // Use the playerInput's current control scheme to get the movement input
            Vector2 inputDirection = playerInput.actions["Move"].ReadValue<Vector2>();

            physics.velocity = inputDirection * movementSpeed;
        }
    }



    void Update()
    {
      
    }
}
