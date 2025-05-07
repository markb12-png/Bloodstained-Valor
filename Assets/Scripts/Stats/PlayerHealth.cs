using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Slider healthSlider;

    [Header("UI")]
    [SerializeField] private DeathUIController deathUIController;

    private float currentHealth;
    private bool isDead = false;

    private Rigidbody2D rb;
    private MonoBehaviour[] playerScripts;

    private Coroutine regenerationCoroutine;
    private float regenerationAmount = 5f;
    private float regenerationInterval = 1f;
    private float delayBeforeRegeneration = 4f;

    private float lastDamageTime;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        rb = GetComponent<Rigidbody2D>();

        playerScripts = GetComponents<MonoBehaviour>()
            .Where(script => script != this)
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

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        StopHealthRegeneration(); // Stop if regenerating

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        lastDamageTime = Time.time; // Reset the timer

        Debug.Log($"[Health] Player took {amount} damage. Current HP: {currentHealth}");

        if (currentHealth <= 0 && !isDead)
        {
            StartCoroutine(HandleDeath());
        }
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

    private System.Collections.IEnumerator RegenerateHealth()
    {
        Debug.Log("[Health] Starting health regeneration.");
        while (currentHealth < maxHealth && !isDead)
        {
            // If damaged during regeneration, stop immediately
            if (Time.time - lastDamageTime < delayBeforeRegeneration)
            {
                Debug.Log("[Health] Regeneration interrupted due to damage.");
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

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }

    private System.Collections.IEnumerator HandleDeath()
    {
        isDead = true;
        Debug.Log("[Health] Player died. Locking movement and restarting in 5 seconds...");

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        foreach (var script in playerScripts)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        if (deathUIController != null)
        {
            deathUIController.TriggerDeathUI();
        }

        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
