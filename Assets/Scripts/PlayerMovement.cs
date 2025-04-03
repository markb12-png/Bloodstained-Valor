using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Normal movement speed
    public float sprintSpeed = 10f; // Sprinting speed
    private Rigidbody2D rb;
    private GroundCheck groundCheck; // Reference to the GroundCheck script

    // Flags to track movement state
    private bool moveLeft = false;
    private bool moveRight = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<GroundCheck>(); // Get the GroundCheck component
    }

    void Update()
    {
        HandleInput(); // Update movement input
    }

    void FixedUpdate()
    {
        ApplyMovement(); // Apply movement physics
    }

    private void HandleInput()
    {
        // Capture movement inputs
        moveLeft = Input.GetKey(KeyCode.A);
        moveRight = Input.GetKey(KeyCode.D);
    }

    private void ApplyMovement()
    {
        // Ensure movement only happens if the player is grounded
        if (!groundCheck.IsGrounded())
        {
            // If not grounded, disable horizontal movement but retain vertical velocity
            rb.velocity = new Vector2(0f, rb.velocity.y);
            return; // Exit movement logic
        }

        // Determine the movement speed
        bool isSprinting = Input.GetKey(KeyCode.LeftShift); // Check sprinting state
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        // Handle horizontal movement
        float horizontalVelocity = 0f;
        if (moveLeft && moveRight)
        {
            horizontalVelocity = 0f; // No movement if both keys are pressed
        }
        else if (moveLeft)
        {
            horizontalVelocity = -currentSpeed; // Move left
        }
        else if (moveRight)
        {
            horizontalVelocity = currentSpeed; // Move right
        }

        // Update Rigidbody2D velocity
        rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y);
    }
}
