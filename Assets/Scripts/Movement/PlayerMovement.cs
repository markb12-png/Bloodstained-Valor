
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Normal movement speed
    public float sprintSpeed = 10f; // Sprinting speed
    public Animator _animator;
    private Rigidbody2D rb;

    // Flags to track movement state
    private bool moveLeft = false;
    private bool moveRight = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Ensure Rigidbody is initialized
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
       
           if( moveLeft = Input.GetKey(KeyCode.A))
        { 
            _animator.SetBool("run", true);
            _animator.SetBool("turn left", true);
        }
        
        if(moveRight = Input.GetKey(KeyCode.D))
        {
            _animator.SetBool("run", true);
            _animator.SetBool("turn left", false);
        }
    }

    private void ApplyMovement()
    {
        // Determine movement speed
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        // Handle horizontal movement
        float horizontalVelocity = 0f;
        if (moveLeft && moveRight)
        {
            horizontalVelocity = 0f; // No movement if both keys are pressed
            _animator.SetBool("idle", false);
        }
        else if (moveLeft)
        {
            _animator.SetBool("idle", false);
            horizontalVelocity = -currentSpeed; // Move left
        }
        else if (moveRight)
        {
            _animator.SetBool("idle", false);
            horizontalVelocity = currentSpeed; // Move right
        }

        // Apply velocity to Rigidbody
        rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y);
        if (horizontalVelocity == 0) 
        {
            _animator.SetBool("idle", true);
            _animator.SetBool("run", false);
        }
    }
}
