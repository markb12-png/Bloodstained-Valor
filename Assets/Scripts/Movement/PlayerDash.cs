using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce = 10f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 1f;
    public int maxDashCharges = 3;
    public float chargeRefillTime = 4f;

    private int currentDashCharges;
    private bool isRefilling = false;
    private bool canDash = true;

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

        currentDashCharges = maxDashCharges;
    }

    private void Update()
    {
        // Check if left or right input is being held
        bool movingInput = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);

        if (Input.GetKeyDown(KeyCode.C) && canDash && currentDashCharges > 0 && movingInput)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        currentDashCharges--;

        if (!isRefilling)
        {
            StartCoroutine(RefillCharges());
        }

        DisableScripts();

        // Determine dash direction based on input
        int inputDirection = Input.GetKey(KeyCode.D) ? 1 : -1;
        Vector2 dashDirection = new Vector2(inputDirection, 0);
        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            float t = elapsedTime / dashDuration;
            float currentForce = Mathf.Lerp(dashForce, 0, t);
            rb.velocity = dashDirection * currentForce;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;

        EnableScripts();

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
        if (jumpScript != null) jumpScript.enabled = false;
        if (attackScript != null) attackScript.enabled = false;
    }

    private void EnableScripts()
    {
        if (movementScript != null) movementScript.enabled = true;
        if (jumpScript != null) jumpScript.enabled = true;
        if (attackScript != null) attackScript.enabled = true;
    }

    public int GetCurrentCharges()
    {
        return currentDashCharges;
    }
}
