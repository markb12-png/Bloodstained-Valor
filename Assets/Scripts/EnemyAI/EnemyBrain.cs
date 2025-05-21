using UnityEngine;

[RequireComponent(typeof(EnemyHealth), typeof(EnemyMovement), typeof(EnemyCombat))]
public class EnemyBrain : MonoBehaviour
{
    public Transform playerPos;

    [Header("Attack Range Detection")]
    [SerializeField] private BoxCollider2D attackRangeCollider;

    private EnemyHealth enemyHealth;
    private EnemyMovement movement;
    private EnemyCombat combat;

    private bool playerInRange = false;
    private Vector2 playerDistance;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        movement = GetComponent<EnemyMovement>();
        combat = GetComponent<EnemyCombat>();
    }

    private void Update()
    {
        if (!playerInRange || enemyHealth.isDead || enemyHealth.isInDeathblowState)
            return;

        playerDistance = playerPos.position - transform.position;
        float xDistance = Mathf.Abs(playerDistance.x);

        // Check if player is inside the attack range box
        if (attackRangeCollider != null && attackRangeCollider.bounds.Contains(playerPos.position))
        {
            if (!combat.IsAttacking && !combat.HasAttacked)
            {
                combat.Attack(playerDistance);
            }
        }
        else
        {
            if (!combat.IsAttacking)
            {
                if (Player.isAttacking && Random.value < 0.5f)
                {
                    movement.Backdash(playerDistance);
                    return;
                }

                if (xDistance > 2.5f) // move closer if too far
                {
                    movement.WalkToward(playerDistance);
                }
                else if (xDistance < 1f) // move back if too close
                {
                    movement.WalkAway(playerDistance);
                }
                else
                {
                    combat.ResetAttackFlag();
                    movement.SetIdleIfStopped();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
