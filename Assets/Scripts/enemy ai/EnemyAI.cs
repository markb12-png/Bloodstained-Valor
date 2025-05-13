using UnityEngine;

public enum EnemyBehaviorState
{
    Offensive,
    Defensive
}

public class EnemyAI : MonoBehaviour
{
    public EnemyBehaviorState behaviorState = EnemyBehaviorState.Offensive;

    public float minDistance = 1.2f;             // Never get closer than this
    public float preferredDistance = 3f;         // Ideal range for defensive
    [Range(0f, 1f)] public float backwalkChance = 0.5f;

    private Transform player;
    private bool playerInRange = false;

    private EnemyForwardMovement forwardMove;
    private EnemyBackwalkMovement backwalkMove;

    private void Start()
    {
        forwardMove = GetComponent<EnemyForwardMovement>();
        backwalkMove = GetComponent<EnemyBackwalkMovement>();
    }

    private void Update()
    {
        if (!playerInRange || player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Global rule: never go inside minDistance
        if (distance <= minDistance)
        {
            StopAllMovement();

            if (behaviorState == EnemyBehaviorState.Defensive && Random.value < backwalkChance)
            {
                backwalkMove.MoveAwayFrom(player.position);
            }

            return;
        }

        // Behavior-specific movement
        switch (behaviorState)
        {
            case EnemyBehaviorState.Offensive:
                forwardMove.MoveTowards(player.position);
                break;

            case EnemyBehaviorState.Defensive:
                if (distance < preferredDistance)
                {
                    StopAllMovement(); // already close enough
                }
                else
                {
                    forwardMove.MoveTowards(player.position); // move in to preferred range
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            StopAllMovement();
        }
    }

    private void StopAllMovement()
    {
        forwardMove.Stop();
        backwalkMove.Stop();
    }
}
