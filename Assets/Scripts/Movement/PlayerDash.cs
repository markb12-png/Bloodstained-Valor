using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce = 10f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 1f;
    public int maxDashCharges = 3;
    public float chargeRefillTime = 4f;

    [Header("UI")]
    [SerializeField] private Slider dashSlider;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip dashRechargeSound;

    private int currentDashCharges;
    private bool isRefilling = false;
    private bool canDash = true;

    private Rigidbody2D rb;
    private Animator animator;

    private PlayerMovement movementScript;
    private PlayerJump jumpScript;
    private PlayerAttack attackScript;

    private int originalLayer;
    [SerializeField] private string invulnerableLayerName = "Invulnerable";

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movementScript = GetComponent<PlayerMovement>();
        jumpScript = GetComponent<PlayerJump>();
        attackScript = GetComponent<PlayerAttack>();

        currentDashCharges = maxDashCharges;
        originalLayer = gameObject.layer;

        UpdateDashSlider();
    }

    private void Update()
    {
        bool movingInput = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)
                         || Input.GetAxisRaw("Horizontal") != 0;

        if ((Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Dash")) && canDash && currentDashCharges > 0 && movingInput)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        currentDashCharges--;
        UpdateDashSlider();

        if (!isRefilling)
        {
            StartCoroutine(RefillCharges());
        }

        DisableScripts();
        gameObject.layer = LayerMask.NameToLayer(invulnerableLayerName);

        if (animator != null)
        {
            animator.Play("knight dash");
        }

        int inputDirection = Input.GetKey(KeyCode.D) || Input.GetAxisRaw("Horizontal") > 0 ? 1 : -1;
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
        gameObject.layer = originalLayer;

        if (animator != null)
        {
            if (rb.velocity.x < 0)
                animator.Play("idle animation left");
            else
                animator.Play("idle animation right");
        }

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
            UpdateDashSlider();

            if (audioSource != null && dashRechargeSound != null)
            {
                audioSource.PlayOneShot(dashRechargeSound);
            }
        }

        isRefilling = false;
    }

    private void UpdateDashSlider()
    {
        if (dashSlider == null) return;
        dashSlider.value = (float)currentDashCharges / maxDashCharges;
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
