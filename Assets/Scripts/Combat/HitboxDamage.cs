using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damage = 10f;

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
            enemy.TakeDamage(damage, transform.position); // Pass hitbox position as hit source!
            Destroy(gameObject);
            return;
        }

        // Handle Player
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage, transform.position); // Already matches your PlayerHealth!
            Destroy(gameObject);
            return;
        }
    }
}
