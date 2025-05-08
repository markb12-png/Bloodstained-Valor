using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Reference to the capsule (or any target to follow)
    public float smoothSpeed = 0.125f; // Adjust this for the smoothness
    public Vector3 offset; // Offset from the target position

    void Start()
    {
        // Set default offset to raise the camera 5 units above the target
        offset = new Vector3(0, 2, 0);
    }

    void FixedUpdate()
    {
        if (target != null) // Ensure the target exists
        {
            // Target position with offset
            Vector3 desiredPosition = target.position + offset;

            // Lock the Z-axis to -10
            desiredPosition.z = -20;

            // Smoothly interpolate the camera's position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Update the camera's position
            transform.position = smoothedPosition;
        }
    }
}
