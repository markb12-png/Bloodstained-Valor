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

    private Animator animator;
    private bool isAttacking = false;
    private bool hasAttacked = false;
    private Vector2 dashDirection;

    public bool IsAttacking => isAttacking;
    public bool HasAttacked => hasAttacked;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Attack(Vector2 directionToPlayer)
    {
        if (isAttacking) return;

        bool facingRight = directionToPlayer.x > 0;
        float originalScale = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(facingRight ? originalScale : -originalScale, transform.localScale.y, transform.localScale.z);
        dashDirection = facingRight ? Vector2.right : Vector2.left;

        // Force play from beginning even if already playing
        animator.Play("enemy crescent slash", -1, 0f);

        isAttacking = true;
        hasAttacked = true;

        Invoke(nameof(SpawnHitbox), hitboxSpawnFrame / 30f);
        Invoke(nameof(FinishAttack), attackAnimationDuration);
    }

    private void SpawnHitbox()
    {
        if (hitboxPrefab == null) return;

        Vector2 spawnPos = (Vector2)transform.position + hitboxOffset * dashDirection;
        GameObject hitbox = Instantiate(hitboxPrefab, spawnPos, Quaternion.identity, transform);

        if (hitbox.TryGetComponent(out HitboxDamage damage))
        {
            damage.SetOwner(gameObject);
        }

        Destroy(hitbox, hitboxDurationFrames / 30f);
    }

    private void FinishAttack()
    {
        isAttacking = false;
        Player.canAttack = true;
        animator.Play("enemy idle");
    }

    public void ResetAttackFlag()
    {
        hasAttacked = false;
    }
}
