using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce = 10f; // Initial strong force for the dash
    public float dashDuration = 0.3f; // Total duration of the dash
    public float dashCooldown = 1f; // Cooldown between dashes
    private Rigidbody2D rb; // Reference to Rigidbody2D

    private PlayerMovement movementScript;
    private PlayerJump jumpScript;
    private PlayerAttack attackScript; // Reference to PlayerAttack script

    private bool canDash = true; // Determines if the player can dash

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Initialize Rigidbody
        movementScript = GetComponent<PlayerMovement>();
        jumpScript = GetComponent<PlayerJump>();
        attackScript = GetComponent<PlayerAttack>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) && canDash) // Dash now uses Left Alt
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false; // Disable further dashes during cooldown

        // Step 1: Disable movement and attack scripts during dash
        DisableScripts();

        // Step 2: Perform the dash based on movement direction
        float elapsedTime = 0f;
        Vector2 dashDirection = rb.velocity.x > 0 ? Vector2.right : Vector2.left;

        while (elapsedTime < dashDuration)
        {
            float t = elapsedTime / dashDuration; // Progression from 0 to 1
            float currentForce = Mathf.Lerp(dashForce, 0, t); // Strong at the start, fades out
            rb.velocity = dashDirection * currentForce; // Apply velocity

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Step 3: Stop movement after dash
        rb.velocity = Vector2.zero;

        // Step 4: Re-enable movement and attack scripts
        EnableScripts();

        // Step 5: Cooldown before next dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true; // Allow dashing again
    }

    private void DisableScripts()
    {
        if (movementScript != null) movementScript.enabled = false;
        if (jumpScript != null) jumpScript.enabled = false;
        if (attackScript != null) attackScript.enabled = false;
    }

    private void EnableScripts()
    {
        if (movementScript != null) movementScript.enabled = true;
        if (jumpScript != null) jumpScript.enabled = true;
        if (attackScript != null) attackScript.enabled = true;
    }
}
