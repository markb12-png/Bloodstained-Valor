using UnityEngine;

public class EnemyBrain : MonoBehaviour
{

    public PlayerHealth playerHealth;

    public Transform playerPos;

    public Vector2 playerDistance;
    public float xDistance;
    public float playerDistanceMin = 1;
    public float playerDistanceOpt = 3;
    public float playerDistanceMax = 8;

    public bool playerDetected;
    public bool hasAttacked;

    public float speed = 0.1f;
    public Vector2 dashDirection;

    private string currentAnimation;
    public Animator animator;

    private float timer = 0f;

    private float defenseChance = 50;

    private enum State { none, offensive, defensive };
    private State state = State.none;


    [Header("Hitbox")]
    [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private Vector2 hitboxOffset = new Vector2(1f, 0);
    [SerializeField] private int hitboxSpawnFrame = 10;
    [SerializeField] private int hitboxDurationFrames = 8;

    public float frame;

    private struct AttackFrames
    {
        public readonly int anticipation;
        public readonly int dash;
        public readonly int recovery;

        public AttackFrames(int startupFrames, int dashFrames, int recoveryFrames)
        {
            anticipation = startupFrames;
            dash = dashFrames;
            recovery = recoveryFrames;
        }
    }

    private AttackFrames sweepFrames = new AttackFrames(9, 6, 12);

    // Add these new fields
    private bool isAttacking = false;
    private float attackAnimationDuration = 0.7f; // Set this to match your animation length in seconds

    #region Logic
    void Update()
    {
        playerDistance = playerPos.position - transform.position;
        xDistance = Mathf.Abs(playerDistance.x);

        int duration = Random.Range(3, 7);
        UpdateState(duration); // every 5 seconds
        switch (state)
        {
            case State.defensive:
                Defensive();
                break;
            case State.offensive:
                Offensive();
                break;
        }
    }
    #endregion


    #region Random
    private void UpdateState(int duration)
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            timer = 0f;
            NewState();
            Debug.Log("State: " + state);
        }
    }

    private void NewState()
    {
        // Generate random value between 0 and 100
        float randomValue = Random.Range(0, 101);
        if (randomValue <= defenseChance)
        {
            state = State.defensive;
            defenseChance -= 20;
        }
        else
        {
            state = State.offensive;
            defenseChance += 20;
        }
    }
    #endregion


    #region Defensive
    private void Defensive()
    {
        if (isAttacking) return; // Don't interrupt attack animation
        
        if (xDistance < playerDistanceMin && !Player.isAttacking)
        {
            Attack();
            if (!isAttacking) // Only walk away if not attacking
            {
                WalkAway();
            }
        }
        else
        {
            // Reset hasAttacked when not in attack range
            hasAttacked = false;
            PlayAnimation("idle animation right");
        }
    }

    private void WalkAway()
    {
        if (playerDistance.x > 0)
        {
            // Player is to the right, so walk left (away)
            PlayAnimation("walk animation");
            transform.position -= new Vector3(speed, 0, 0);
        }
        else if (playerDistance.x < 0)
        {
            // Player is to the left, so walk right (away)
            PlayAnimation("walk animation flipped");
            transform.position += new Vector3(speed, 0, 0);
        }
        else
        {
            // Player is directly above/below, pick a direction (default: right)
            PlayAnimation("walk animation flipped");
            transform.position += new Vector3(speed, 0, 0);
        }
    }
    #endregion


    #region Offensive
    private void Offensive()
    {
        if (isAttacking) return; // Don't interrupt attack animation
        
        if (xDistance < playerDistanceMax && !Player.isAttacking)
        {
            if (xDistance <= playerDistanceOpt && !hasAttacked)
            {
                // When in optimal attack range
                Attack();
            }
            else if (!isAttacking) // Only walk toward if not attacking
            {
                WalkToward();
            }
        }
        else 
        {
            PlayAnimation("idle animation right");
            // Reset attack state when out of range
            hasAttacked = false;
        }
    }

    private void WalkToward()
    {
        if (playerDistance.x > playerDistanceOpt && !Player.isAttacking)
        {
            // Player is to the right, so walk right (toward)
            PlayAnimation("walk animation");
            transform.position += new Vector3(speed, 0, 0);
        }
        else if (playerDistance.x < -playerDistanceOpt && !Player.isAttacking)
        {
            // Player is to the left, so walk left (toward)
            PlayAnimation("walk animation flipped");
            transform.position -= new Vector3(speed, 0, 0);
        }
        else
        {
            PlayAnimation("idle animation right");
        }
    }
    #endregion


    #region Attacks
    private void Attack()
    {
        if (!hasAttacked && !isAttacking)
        {
            // Determine direction based on player position
            bool facingRight = playerDistance.x > 0;
            transform.localScale = new Vector3(facingRight ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            
            dashDirection = facingRight ? Vector2.right : Vector2.left;
            Sweep();
            hasAttacked = true;
            isAttacking = true;
            
            // Reset isAttacking flag after animation finishes
            Invoke("FinishAttack", attackAnimationDuration);
        }
    }

    private void FinishAttack()
    {
        isAttacking = false;
        Player.canAttack = true;
    }

    private void Sweep()
    {
        // Play sweep attack animation
        PlayAnimation("knight sword attack"); // Your attack animation
        
        // We could use a coroutine or animation events for better timing
        Invoke("SpawnHitboxWithDelay", hitboxSpawnFrame / 30f); // Convert frames to seconds
    }

    private void SpawnHitboxWithDelay()
    {
        SpawnHitbox(dashDirection);
    }

    private void SpawnHitbox(Vector2 direction)
    {
        var shake = Camera.main?.GetComponent<CameraShake>();
        if (shake != null) shake.StartShake(0.1f, 0.2f);

        if (hitboxPrefab == null) return;

        Vector2 spawnPosition = (Vector2)transform.position + hitboxOffset * direction;
        GameObject hitbox = Instantiate(hitboxPrefab, spawnPosition, Quaternion.identity, transform);
        HitboxDamage damage = hitbox.GetComponent<HitboxDamage>();
        if (damage != null) damage.SetOwner(gameObject);

        Destroy(hitbox, hitboxDurationFrames / 30f);
    }
    #endregion

    #region Animations
    private void PlayAnimation(string animName)
    {
        // Don't interrupt attack animations with other animations
        if (isAttacking && currentAnimation == "knight sword attack" && animName != "knight sword attack") 
            return;
            
        if (currentAnimation == animName) return;

        animator.Play(animName);
        currentAnimation = animName;
    }

    private void HandleAnimations()
    {
        
    }

    #endregion
}
