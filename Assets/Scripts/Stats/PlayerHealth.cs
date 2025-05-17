using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Slider healthSlider;

    [Header("Regen Settings")]
    [SerializeField] private float regenerationAmount = 5f;
    [SerializeField] private float regenerationInterval = 1f;
    [SerializeField] private float delayBeforeRegeneration = 4f;

    [Header("Death UI")]
    [SerializeField] private GameObject deathOverlay;
    [SerializeField] private GameObject deathText;

    private float currentHealth;
    private bool isDead = false;
    private float lastDamageTime;
    private Coroutine regenerationCoroutine;

    private Rigidbody2D rb;
    private Animator animator;
    private MonoBehaviour[] playerScripts;

    private void Awake()
    {
        // Guaranteed: Hide the death UI before anything else
        if (deathOverlay != null) deathOverlay.SetActive(false);
        if (deathText != null) deathText.SetActive(false);
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        playerScripts = GetComponents<MonoBehaviour>()
            .Where(script => script != this)
            .ToArray();
    }

    private void Update()
    {
        if (!isDead && currentHealth < maxHealth)
        {
            if (Time.time - lastDamageTime >= delayBeforeRegeneration && regenerationCoroutine == null)
            {
                regenerationCoroutine = StartCoroutine(RegenerateHealth());
            }
        }
    }

    public void TakeDamage(float amount, Vector2 hitSourcePosition)
    {
        if (isDead) return;

        StopHealthRegeneration();

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        lastDamageTime = Time.time;

        // Knockback from hit direction
        Vector2 knockbackDir = (transform.position - (Vector3)hitSourcePosition).normalized;
        rb.AddForce(knockbackDir * 10f, ForceMode2D.Impulse);

        // Camera shake
        var shake = Camera.main?.GetComponent<CameraShake>();
        if (shake != null)
        {
            shake.StartShake(0.3f, 0.2f);
        }

        if (currentHealth > 0)
        {
            animator.Play("knight stun");
            StartCoroutine(StunRoutine());
        }
        else
        {
            isDead = true;
            StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator StunRoutine()
    {
        var groundDetector = GetComponent<GroundDetector>();
        var toDisable = GetComponents<MonoBehaviour>()
            .Where(m => m != this && m != groundDetector)
            .ToList();

        foreach (var script in toDisable)
            script.enabled = false;

        yield return new WaitForSeconds(0.5f);

        if (rb.velocity.x < 0)
            animator.Play("idle animation left");
        else
            animator.Play("idle animation right");

        foreach (var script in toDisable)
            script.enabled = true;

        // FIX: Reset jump state so the player can jump after stun
        var jump = GetComponent<PlayerJump>();
        if (jump != null)
            jump.ResetJumpState();
    }


    private IEnumerator HandleDeath()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        foreach (var script in playerScripts)
        {
            if (script != null)
                script.enabled = false;
        }

        if (animator != null)
        {
            animator.Play("knight death");
        }

        // Activate death UI elements on death
        if (deathOverlay != null) deathOverlay.SetActive(true);
        if (deathText != null) deathText.SetActive(true);

        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator RegenerateHealth()
    {
        Debug.Log("[Health] Starting health regeneration.");
        while (currentHealth < maxHealth && !isDead)
        {
            if (Time.time - lastDamageTime < delayBeforeRegeneration)
            {
                regenerationCoroutine = null;
                yield break;
            }

            currentHealth += regenerationAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UpdateHealthUI();

            yield return new WaitForSeconds(regenerationInterval);
        }

        Debug.Log("[Health] Regeneration complete.");
        regenerationCoroutine = null;
    }

    private void StopHealthRegeneration()
    {
        if (regenerationCoroutine != null)
        {
            StopCoroutine(regenerationCoroutine);
            regenerationCoroutine = null;
            Debug.Log("[Health] Regeneration stopped.");
        }
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }
}
