using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce = 10f; // Initial strong force for the dash
    public float dashDuration = 0.3f; // Total duration of the dash
    public float dashCooldown = 1f; // Cooldown between dashes
    public int maxDashCharges = 3; // Maximum number of dash charges
    public float chargeRefillTime = 4f; // Time to refill one dash charge

    private int currentDashCharges;
    private bool isRefilling = false;
    private bool canDash = true; // Controls cooldown between dashes

    private Rigidbody2D rb;

    private PlayerMovement movementScript;
    private PlayerJump jumpScript;
    private PlayerAttack attackScript;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<PlayerMovement>();
        jumpScript = GetComponent<PlayerJump>();
        attackScript = GetComponent<PlayerAttack>();

        currentDashCharges = maxDashCharges; // Start fully charged
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && canDash && currentDashCharges > 0)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;           // Start cooldown between dashes
        currentDashCharges--;      // Use one charge

        if (!isRefilling)
        {
            StartCoroutine(RefillCharges());
        }

        // Disable other scripts during dash
        DisableScripts();

        // Determine dash direction
        Vector2 dashDirection = rb.velocity.x > 0 ? Vector2.right : Vector2.left;
        float elapsedTime = 0f;

        // Dash movement loop
        while (elapsedTime < dashDuration)
        {
            float t = elapsedTime / dashDuration;
            float currentForce = Mathf.Lerp(dashForce, 0, t);
            rb.velocity = dashDirection * currentForce;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Stop horizontal velocity after dash
        rb.velocity = Vector2.zero;

        // Re-enable scripts after dash
        EnableScripts();

        // Wait for dash cooldown before next dash (if charges remain)
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private IEnumerator RefillCharges()
    {
        isRefilling = true;

        while (currentDashCharges < maxDashCharges)
        {
            yield return new WaitForSeconds(chargeRefillTime);
            currentDashCharges++;
            Debug.Log($"Dash charge refilled. Current charges: {currentDashCharges}");
        }

        isRefilling = false;
    }

    private void DisableScripts()
    {
        if (movementScript != null) movementScript.enabled = false;
        if (jumpScript != null) movementScript.enabled = false;
        if (attackScript != null) attackScript.enabled = false;
    }

    private void EnableScripts()
    {
        if (movementScript != null) movementScript.enabled = true;
        if (jumpScript != null) movementScript.enabled = true;
        if (attackScript != null) attackScript.enabled = true;
    }

    // Optional: Use this for a UI or display
    public int GetCurrentCharges()
    {
        return currentDashCharges;
    }
}
