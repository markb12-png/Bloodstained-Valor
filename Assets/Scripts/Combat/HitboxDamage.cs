using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 25f;

    private GameObject owner;            // The one who created the hitbox
    private bool hasDealtDamage = false; // Prevent multiple hits

    // Called by the summoner to declare ownership
    public void SetOwner(GameObject creator)
    {
        owner = creator;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasDealtDamage) return;

        // Ignore the object that spawned the hitbox
        if (other.gameObject == owner)
        {
            Debug.Log("[HitboxDamage] Ignoring collision with owner.");
            return;
        }

        Debug.Log($"[HitboxDamage] OnTriggerEnter2D with: {other.gameObject.name}");

        var health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            Debug.Log($"[HitboxDamage] Dealing {damageAmount} damage to: {other.gameObject.name}");
            health.TakeDamage(damageAmount);
            hasDealtDamage = true;
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Debug.Log($"[HitboxDamage] Destroyed: {gameObject.name}");
    }
}
