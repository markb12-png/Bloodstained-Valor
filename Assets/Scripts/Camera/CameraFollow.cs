using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void Start() {
        // Set default offset to raise the camera above the target
        offset = new Vector3(0, 2, 0);
    }

    void FixedUpdate() {
        if (target != null) {
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
