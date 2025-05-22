using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    private GameObject player;
    private EnemyMovement enemyMovement;
    private EnemyCombat enemyCombat;
    private bool playerInRange = false;

    public bool enemyIsAlerted => playerInRange;

    [SerializeField] private Collider2D attackRangeCollider;

    private readonly float lockedScale = 0.75f;
    private PlayerAttack playerAttack;
    private PlayerAirAttack playerAirAttack;
    private System.Random rng = new System.Random();

    private bool hasTriggeredFogwall = false;
    private bool didEvaluateBackdash = false;

    private void Start()
    {
        PlayerHealth.PlayerIsDead = false;
        player = GameObject.Find("Player");
        enemyMovement = GetComponent<EnemyMovement>();
        enemyCombat = GetComponent<EnemyCombat>();

    

        playerAttack = player.GetComponent<PlayerAttack>();
        playerAirAttack = player.GetComponent<PlayerAirAttack>();

        if (playerAttack == null || playerAirAttack == null)
            Debug.LogError("[EnemyBrain] PlayerAttack or PlayerAirAttack script not found on Player.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            if (!hasTriggeredFogwall)
            {
                hasTriggeredFogwall = true;

                if (gameObject.name.Contains("1"))
                    GameObject.Find("Fogwall 1")?.SetActive(true);
                else if (gameObject.name.Contains("2"))
                    GameObject.Find("Fogwall 2")?.SetActive(true);
            }
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            playerInRange = false;
            Debug.Log("[EnemyBrain] Player EXITED detection zone.");
        }
    }

    private void Update()
    {
        if (PlayerHealth.PlayerIsDead || player == null) return;

        if (
            (playerAttack != null && !playerAttack.IsPlayerAttacking()) &&
            (playerAirAttack != null && !playerAirAttack.IsAirAttacking()))
        {
            didEvaluateBackdash = false;
        }

        if (playerInRange)
        {
            float direction = player.transform.position.x - transform.position.x;
            transform.localScale = new Vector3(
                direction > 0 ? lockedScale : -lockedScale,
                lockedScale,
                lockedScale
            );

            if (!didEvaluateBackdash &&
                ((playerAttack != null && playerAttack.IsPlayerAttacking()) ||
                (playerAirAttack != null && playerAirAttack.IsAirAttacking())))
            {
                didEvaluateBackdash = true;
                double randomvalue = rng.NextDouble();
                if (randomvalue < 0.5)
                {
                    Vector2 directionFromPlayer = transform.position - player.transform.position;
                    enemyMovement.Backdash(directionFromPlayer);
                    return;
                }
            }

            if (!attackRangeCollider.bounds.Intersects(player.GetComponent<Collider2D>().bounds))
            {
                enemyMovement.MoveToward(player.transform.position.x);
            }
            else
            {
                enemyMovement.Stop();

                if (!enemyCombat.IsAttacking())
                {
                    if (rng.NextDouble() < 0.5)
                    {
                        enemyCombat.Attack();
                    }
                }
            }
        }
        else
        {
            enemyMovement.Stop();
        }
    }
}
