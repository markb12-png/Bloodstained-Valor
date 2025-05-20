using UnityEngine;

[RequireComponent(typeof(EnemyMovement), typeof(EnemyCombat))]
public class EnemyBrain : MonoBehaviour
{
    public Transform playerPos;
    private EnemyHealth enemyHealth;
    private EnemyMovement movement;
    private EnemyCombat combat;

    public float playerDistanceMin = 1;
    public float playerDistanceOpt = 3;
    public float playerDistanceMax = 8;

    private float xDistance;
    private Vector2 playerDistance;

    private float timer = 0f;
    private float defenseChance = 50;
    private enum State { none, offensive, defensive }
    private State state = State.none;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        movement = GetComponent<EnemyMovement>();
        combat = GetComponent<EnemyCombat>();
    }

    private void Update()
    {
        if (enemyHealth.isDead || enemyHealth.isInDeathblowState) return;

        playerDistance = playerPos.position - transform.position;
        xDistance = Mathf.Abs(playerDistance.x);

        UpdateState(Random.Range(3, 7));

        switch (state)
        {
            case State.defensive:
                if (!combat.IsAttacking)
                {
                    if (xDistance < playerDistanceMin && !Player.isAttacking)
                    {
                        combat.Attack(playerDistance);
                        movement.WalkAway(playerDistance);
                    }
                    else
                    {
                        combat.ResetAttackFlag();
                    }
                }
                break;

            case State.offensive:
                if (!combat.IsAttacking)
                {
                    if (xDistance < playerDistanceMax && !Player.isAttacking)
                    {
                        if (xDistance <= playerDistanceOpt && !combat.HasAttacked)
                        {
                            combat.Attack(playerDistance);
                        }
                        else
                        {
                            movement.WalkToward(playerDistance);
                        }
                    }
                    else
                    {
                        combat.ResetAttackFlag();
                    }
                }
                break;
        }
    }

    private void UpdateState(int duration)
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            timer = 0f;
            state = Random.Range(0f, 100f) <= defenseChance ? State.defensive : State.offensive;
            defenseChance = state == State.defensive ? defenseChance - 20 : defenseChance + 20;
        }
    }
}