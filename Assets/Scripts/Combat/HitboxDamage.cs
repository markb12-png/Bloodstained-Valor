using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damage = 10f;

    // Used by clash system to identify hitbox source
    public GameObject Owner { get; private set; }
    public bool ClashTriggered { get; set; }

    /// <summary> Assigns the GameObject that created this hitbox. </summary>
    public void SetOwner(GameObject newOwner)
    {
        Owner = newOwner;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Prevent hitting the creator of the hitbox
        if (other.gameObject == Owner)
            return;

        // Deal damage to Enemy
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject); // Destroy hitbox after applying damage
            return;
        }

        // Deal damage to Player with knockback
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage, transform.position); // Pass hitbox position
            Destroy(gameObject); // Destroy hitbox after applying damage
            return;
        }

        // You can add additional logic for breakables, parry shields, etc. here
    }
}
