using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce = 10f; // Initial strong force for the dash
    public float dashDuration = 0.3f; // Total duration of the dash (seconds)
    public float dashCooldown = 1f; // Cooldown between dashes (seconds)
    private Rigidbody2D rb; // Reference to Rigidbody2D

    private PlayerMovement movementScript;
    private PlayerJump jumpScript;
    private PlayerAttack attackScript; // Reference to PlayerAttack script
    private GroundCheck groundCheck;

    private bool canDash = true; // Determines if the player can dash
    private bool isFacingRight = true; // Tracks the direction the player is facing

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<GroundCheck>();

        // Get references to other movement-related scripts
        movementScript = GetComponent<PlayerMovement>();
        jumpScript = GetComponent<PlayerJump>();
        attackScript = GetComponent<PlayerAttack>(); // Get the PlayerAttack script
    }

    void Update()
    {
        // Update facing direction based on velocity
        UpdateFacingDirection();

        // Trigger the dash if grounded, not on cooldown, and Left Alt is pressed
        if (Input.GetKeyDown(KeyCode.LeftAlt) && canDash && groundCheck.IsGrounded())
        {
            StartCoroutine(Dash());
        }
    }

    private void UpdateFacingDirection()
    {
        // Determine direction based on the player's velocity
        if (rb.velocity.x > 0.01f)
        {
            isFacingRight = true;
        }
        else if (rb.velocity.x < -0.01f)
        {
            isFacingRight = false;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false; // Disable further dashes during cooldown

        // Step 1: Disable movement and attack scripts during dash
        DisableScripts();

        // Step 2: Perform the dash with a fast-to-slow effect
        float elapsedTime = 0f;
        Vector2 dashDirection = isFacingRight ? Vector2.right : Vector2.left;

        while (elapsedTime < dashDuration)
        {
            // Apply ease-out effect (start strong, then taper off)
            float t = elapsedTime / dashDuration; // Progression from 0 to 1
            float currentForce = Mathf.Lerp(dashForce, 0, t); // Strong at the start, fades out
            rb.velocity = dashDirection * currentForce; // Apply calculated velocity

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Step 3: Stop movement after the dash
        rb.velocity = Vector2.zero;

        // Step 4: Re-enable movement and attack scripts after dash ends
        EnableScripts();

        // Step 5: Cooldown period before next dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true; // Allow dashing again
    }

    private void DisableScripts()
    {
        // Disable all movement-related scripts
        if (movementScript != null) movementScript.enabled = false;
        if (jumpScript != null) jumpScript.enabled = false;

        // Disable attack script
        if (attackScript != null) attackScript.enabled = false;
    }

    private void EnableScripts()
    {
        // Re-enable all movement-related scripts
        if (movementScript != null) movementScript.enabled = true;
        if (jumpScript != null) jumpScript.enabled = true;

        // Re-enable attack script
        if (attackScript != null) attackScript.enabled = true;
    }
}
