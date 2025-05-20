using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed = 0.1f;
    private Animator animator;
    private Rigidbody2D rb;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void WalkToward(Vector2 directionToPlayer)
    {
        float dir = Mathf.Sign(directionToPlayer.x);
        transform.position += new Vector3(speed * dir, 0, 0);
        animator.Play("enemy forwards walk");
    }

    public void WalkAway(Vector2 directionToPlayer)
    {
        float dir = Mathf.Sign(directionToPlayer.x);
        transform.position -= new Vector3(speed * dir, 0, 0);
        animator.Play("enemy backwards walk");
    }

    public void SetIdleIfStopped()
    {
        if (rb.velocity.magnitude < 0.01f)
        {
            animator.Play("enemy idle");
        }
    }
}
