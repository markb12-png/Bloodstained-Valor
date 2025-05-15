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

    [Header("UI")]
    [SerializeField] private DeathUIController deathUIController;

    [Header("Stun Settings")]
    [SerializeField] private float stunDuration = 0.5f;
    [SerializeField] private float knockbackForce = 15f;

    private float currentHealth;
    private bool isDead = false;

    private Rigidbody2D rb;
    private Animator animator;
    private MonoBehaviour[] allScripts;
    private GroundDetector groundDetector;

    private Coroutine regenerationCoroutine;
    private float regenerationAmount = 5f;
    private float regenerationInterval = 1f;
    private float delayBeforeRegeneration = 4f;

    private float lastDamageTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        groundDetector = GetComponent<GroundDetector>();

        currentHealth = maxHealth;
        UpdateHealthUI();

        allScripts = GetComponents<MonoBehaviour>()
            .Where(script => script != this && !(script is GroundDetector))
            .ToArray();

        Debug.Log($"[Health] Player initialized with {currentHealth} HP.");
    }

    void Update()
    {
        if (!isDead && currentHealth < maxHealth)
        {
            if (Time.time - lastDamageTime >= delayBeforeRegeneration && regenerationCoroutine == null)
            {
                regenerationCoroutine = StartCoroutine(RegenerateHealth());
            }
        }

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            StopHealthRegeneration();
            StartCoroutine(HandleDeath());
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

        // Knockback direction: away from hitbox
        Vector2 knockbackDir = (transform.position - (Vector3)hitSourcePosition).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        // Camera shake (optional)
        var shake = Camera.main?.GetComponent<CameraShake>();
        if (shake != null) shake.Shake();

        // Log
        Debug.Log($"[Health] Player took {amount} damage. Current HP: {currentHealth}");

        if (currentHealth > 0)
        {
            StartCoroutine(Stun());
        }
        else if (!isDead)
        {
            isDead = true;
            StopHealthRegeneration();
            StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator Stun()
    {
        Debug.Log("[Health] Stunned.");
        animator.Play("knight stun");
        ToggleOtherScripts(false);
        yield return new WaitForSeconds(stunDuration);
        ToggleOtherScripts(true);
    }

    private IEnumerator HandleDeath()
    {
        Debug.Log("[Health] Player died. Playing death animation.");
        animator.Play("knight death animation");

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        ToggleOtherScripts(false);

        if (deathUIController != null)
        {
            deathUIController.TriggerDeathUI();
        }

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
                Debug.Log("[Health] Regeneration interrupted.");
                regenerationCoroutine = null;
                yield break;
            }

            currentHealth += regenerationAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UpdateHealthUI();

            yield return new WaitForSeconds(regenerationInterval);
        }

        regenerationCoroutine = null;
    }

    private void StopHealthRegeneration()
    {
        if (regenerationCoroutine != null)
        {
            StopCoroutine(regenerationCoroutine);
            regenerationCoroutine = null;
        }
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }

    private void ToggleOtherScripts(bool state)
    {
        foreach (var script in allScripts)
        {
            if (script != null) script.enabled = state;
        }
    }
}
