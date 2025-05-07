using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 25f;  // Damage amount

    private bool hasDealtDamage = false;  // Prevent multiple hits

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasDealtDamage) return;

        Debug.Log($"[HitboxDamage] OnTriggerEnter2D with: {other.gameObject.name}");

        var health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            Debug.Log($"[HitboxDamage] Dealing {damageAmount} damage to: {other.gameObject.name}");
            health.TakeDamage(damageAmount);
            hasDealtDamage = true;
            Destroy(gameObject);  // Remove hitbox immediately after hitting
        }
    }

    private void OnDestroy()
    {
        Debug.Log($"[HitboxDamage] Destroyed: {gameObject.name}");
    }
}
