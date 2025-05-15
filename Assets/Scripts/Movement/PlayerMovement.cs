using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public Animator animator;

    private Rigidbody2D rb;
    private string currentAnimation = "";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        HandleAnimation();
    }

    private void HandleMovement()
    {
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.A)) moveInput -= 1f;
        if (Input.GetKey(KeyCode.D)) moveInput += 1f;

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);
    }

    private void HandleAnimation()
    {
        float horizontalSpeed = Mathf.Abs(rb.velocity.x);
        float direction = Mathf.Sign(rb.velocity.x);

        // If moving, play movement animations and skip idle
        if (horizontalSpeed > 0f)
        {
            if (Mathf.Approximately(horizontalSpeed, walkSpeed))
            {
                if (direction > 0)
                    PlayAnimation("walk animation");
                else
                    PlayAnimation("walk animation flipped");
            }
            else if (Mathf.Approximately(horizontalSpeed, runSpeed))
            {
                if (direction > 0)
                    PlayAnimation("run animation");
                else
                    PlayAnimation("run animations flipped");
            }

            return; // skip idle logic
        }

        // If completely still, play idle
        PlayAnimation("idle animation right");
    }

    private void PlayAnimation(string animName)
    {
        if (currentAnimation == animName) return;

        animator.Play(animName);
        currentAnimation = animName;
    }
}
