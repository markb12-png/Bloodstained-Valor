using UnityEngine;

public class FireHitbox : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damage = 20f;

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
            return;
        }
    }
}
