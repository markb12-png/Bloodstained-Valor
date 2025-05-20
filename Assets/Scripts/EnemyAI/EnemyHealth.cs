using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private Slider healthSlider;

    [Header("Stun Settings")]
    [SerializeField] private float stunDuration = 0.5f;
    [SerializeField] private float knockbackForce = 10f;

    private float currentHealth;
    public bool isDead = false;
    public bool closeToPlayer = false;

    private MonoBehaviour[] enemyScripts;
    private Rigidbody2D rb;
    private Animator animator; // <-- Add this

    private void Awake()
    {
        if (healthSlider != null) healthSlider.gameObject.SetActive(true);
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // <-- Get Animator

        enemyScripts = GetComponents<MonoBehaviour>()
            .Where(script => script != this)
            .ToArray();
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag == "Player") closeToPlayer = true;
        else closeToPlayer = false;
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Player") closeToPlayer = false;
    }

    // Requires both amount and hitSourcePosition!
    public void TakeDamage(float amount, Vector2 hitSourcePosition)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        // Knockback: always away from attacker, with a slight upward angle
        if (rb != null)
        {
            float direction = Mathf.Sign(transform.position.x - hitSourcePosition.x);
            if (closeToPlayer) direction = -direction;
            Vector2 knockbackDir = new Vector2(direction, 0.3f).normalized;
            rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        }

        if (currentHealth > 0)
        {
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
        var toDisable = GetComponents<MonoBehaviour>()
            .Where(m => m != this)
            .ToList();

        foreach (var script in toDisable)
            script.enabled = false;

        yield return new WaitForSeconds(stunDuration);

        foreach (var script in toDisable)
            script.enabled = true;
    }

    private IEnumerator HandleDeath()
    {
        isDead = true;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        foreach (var script in enemyScripts)
        {
            if (script != null)
                script.enabled = false;
        }

        // --- PLAY THE DEATH ANIMATION ---
        if (animator != null)
        {
            animator.Play("knight death");
        }

        if (healthSlider != null)
            healthSlider.gameObject.SetActive(false);

        // Optionally wait for the animation to finish (uncomment below if desired):
        // yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Destroy(gameObject); // Uncomment if you want the enemy to disappear
        yield return null;
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }
}
