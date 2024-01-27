using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public Rigidbody2D physics;
    
    private Vector2 moveDirection = Vector2.zero;

    void Start()
    {
    }

    private void FixedUpdate()
    {
        Move();
        Vector3 pos;
        Quaternion rot;
        //transform.GetPositionAndRotation(out pos, out rot);
        //pos.z = 0;
        //transform.SetPositionAndRotation(pos, rot);
    }

    public void OnMove(InputValue value)
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
