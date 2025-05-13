using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    [SerializeField] private float damage = 10f;

    private GameObject owner;

    public void SetOwner(GameObject newOwner)
    {
        owner = newOwner;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Prevent hitting the one who spawned it
        if (other.gameObject == owner) return;

        // Check if target has EnemyHealth
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log("[Hitbox] Damaged enemy.");
        }

        // Optional: also damage player
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Debug.Log("[Hitbox] Damaged player.");
        }
    }
}
