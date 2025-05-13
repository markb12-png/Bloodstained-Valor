using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyForwardMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Animator animator;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void MoveTowards(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
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
