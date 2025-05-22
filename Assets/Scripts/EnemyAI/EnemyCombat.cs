using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private Vector2 hitboxOffset = new Vector2(1f, 0);
    [SerializeField] private int hitboxSpawnFrame = 10;
    [SerializeField] private int hitboxDurationFrames = 8;
    [SerializeField] private float attackAnimationDuration = 0.7f;
    [SerializeField] private float attackCooldown = 1.5f;

    private Animator animator;
    private int currentFrame = 0;
    private float timer = 0f;
    private GameObject brain;
    private GameObject movement;
    private bool isOnCooldown = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        brain = GetComponent<EnemyBrain>()?.gameObject;
        movement = GetComponent<EnemyMovement>()?.gameObject;
    }

    public bool IsAttacking()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("enemy crescent slash");
    }

    // Now public!
    public IEnumerator EnemyAttack()
    {
        // If already attacking or on cooldown, exit coroutine early
        if (IsAttacking() || isOnCooldown) yield break;

        isOnCooldown = true;

        animator.Play("enemy crescent slash", -1, 0f);

        if (brain != null) brain.GetComponent<EnemyBrain>().enabled = false;
        if (movement != null) movement.GetComponent<EnemyMovement>().enabled = false;

        // Wait until it's time to spawn the hitbox (converted frames to seconds)
        float hitboxDelay = hitboxSpawnFrame / 30f;
        yield return new WaitForSeconds(hitboxDelay);

        SpawnHitbox();

        // Wait until attack animation is finished (duration - hitboxDelay)
        float animationRemainder = attackAnimationDuration - hitboxDelay;
        if (animationRemainder > 0)
            yield return new WaitForSeconds(animationRemainder);

        FinishAttack();

        // Restore movement/brain (optional: do it in FinishAttack if you want)
        if (brain != null) brain.GetComponent<EnemyBrain>().enabled = true;
        if (movement != null) movement.GetComponent<EnemyMovement>().enabled = true;

        // Wait out the cooldown before another attack can start
        yield return new WaitForSeconds(attackCooldown - attackAnimationDuration);

        isOnCooldown = false;
    }

    // Added for convenience
    public void Attack()
    {
        StartCoroutine(EnemyAttack());
    }

    private void SpawnHitbox()
    {
        if (hitboxPrefab == null) return;

        float direction = Mathf.Sign(transform.localScale.x);
        Vector2 spawnPos = (Vector2)transform.position + hitboxOffset * direction;
        GameObject hitbox = Instantiate(hitboxPrefab, spawnPos, Quaternion.identity, transform);

        if (hitbox.TryGetComponent(out HitboxDamage damage))
        {
            damage.SetOwner(gameObject);
        }

        Destroy(hitbox, hitboxDurationFrames / 30f);
    }

    private void FinishAttack()
    {
        if (brain != null) brain.GetComponent<EnemyBrain>().enabled = true;
        if (movement != null) movement.GetComponent<EnemyMovement>().enabled = true;

        animator.Play("enemy idle");
    }

    private void ResetCooldown()
    {
        isOnCooldown = false;
    }
}
