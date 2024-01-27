using System;
using System.Collections; 
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public Rigidbody2D physics;
    public InputAction playerControls;

    private Vector2 moveDirection = Vector2.zero;
    
    void Start()
    {
        
    }

    void Update()
    {
        
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
        //physics.AddForce(moveDirection * movementSpeed);
        physics.velocity = moveDirection * movementSpeed;
    }
}
