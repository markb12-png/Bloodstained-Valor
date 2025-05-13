using System.Linq;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;
    private bool isDead = false;

    private MonoBehaviour[] enemyScripts;
    private Rigidbody2D rb;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        enemyScripts = GetComponents<MonoBehaviour>()
            .Where(script => script != this)
            .ToArray();

        Debug.Log($"[EnemyHealth] Initialized with {currentHealth} HP.");
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"[EnemyHealth] Took {amount} damage. Current HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[EnemyHealth] Enemy died.");

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        foreach (var script in enemyScripts)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        // Optional: destroy or pool enemy object
        // Destroy(gameObject);
    }
}
