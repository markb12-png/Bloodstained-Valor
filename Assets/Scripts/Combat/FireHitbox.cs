using UnityEngine;

public class FireHitbox : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float knockbackForce = 8f; // Add this line

    public GameObject Owner { get; private set; }
    public bool ClashTriggered { get; set; }

    public void SetOwner(GameObject newOwner)
    {
        Owner = newOwner;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == Owner) return;
        if (ClashTriggered) return;

        // Handle Enemy
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, transform.position);
            return;
        }

        // Handle Player
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage, transform.position);

            // Knockback
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (player.transform.position - transform.position).normalized;
                rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            }

            return;
        }
    }
}
