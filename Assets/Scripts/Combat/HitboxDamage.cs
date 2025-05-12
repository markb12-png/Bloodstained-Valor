using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 25f;

    private GameObject owner;
    private bool hasDealtDamage = false;

    public GameObject Owner => owner;
    public bool ClashTriggered { get; set; } = false;

    public void SetOwner(GameObject creator) => owner = creator;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasDealtDamage || ClashTriggered) return;
        if (other.gameObject == owner) return;

        var health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damageAmount);
            hasDealtDamage = true;
            Destroy(gameObject);
        }
    }
}
