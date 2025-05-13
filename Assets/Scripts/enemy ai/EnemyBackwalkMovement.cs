using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBackwalkMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // Slightly slower than forward
    public Animator animator;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void MoveAwayFrom(Vector2 targetPosition)
    {
        Vector2 direction = ((Vector2)transform.position - targetPosition).normalized;
        Vector2 velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        rb.velocity = velocity;

        UpdateAnimation(direction.x);
    }

    public void Stop()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        animator.SetBool("run", false);
        animator.SetBool("idle", true);
    }

    private void UpdateAnimation(float directionX)
    {
        animator.SetBool("run", true);
        animator.SetBool("idle", false);
        animator.SetBool("turn left", directionX < 0);
    }
}
