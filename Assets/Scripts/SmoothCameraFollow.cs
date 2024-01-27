using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;
    public float damping = 0.2f;

    private Vector3 velocity = Vector3.zero;
    
    void Update()
    {
        if (target)
        {
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = target.position;
            targetPosition.z = currentPosition.z;
            transform.position = Vector3.SmoothDamp(currentPosition, targetPosition, ref velocity, damping);
        }
    }
}
