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
                playerHealth.TakeDamage(damageAmount);
            }
            else
            {
                Debug.LogWarning("[Damage] No PlayerHealth script assigned!");
            }
        }
    }
}
