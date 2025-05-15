using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private PlayerHealth playerHealth;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerHealth != null)
            {
                Debug.Log("[Damage] E key pressed. Damaging player...");

                // Use this object's position as the source of damage
                playerHealth.TakeDamage(damageAmount, transform.position);
            }
            else
            {
                Debug.LogWarning("[Damage] No PlayerHealth script assigned!");
            }
        }
    }
}
