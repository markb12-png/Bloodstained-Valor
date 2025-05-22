using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float speed;

    public Animator animator;
    public Rigidbody2D rb;

    private string currentAnimation;

    void Update()
    {
        HandleMovement();

        // Use Input Manager for sprint
        if (Input.GetButton("Sprint"))
        {
            speed = runSpeed;
        }
        else
        {
            speed = walkSpeed;
        }

        HandleAnimation();
    }

    private void HandleMovement()
    {
        // Use horizontal axis from Input Manager (supports keyboard + controller)
        float move = Input.GetAxisRaw("Horizontal"); // Returns -1, 0, or 1
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
    }

    private void HandleAnimation()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float horizontalSpeed = Mathf.Abs(rb.velocity.x);
        float direction = Mathf.Sign(rb.velocity.x);

        // Force override to walk/run if moving, always override idle if velocity present
        if (horizontalInput != 0 && horizontalSpeed > 0.05f)
        {
            bool isRunning = speed == runSpeed;

            if (direction > 0)
            {
                if (isRunning)
                    PlayAnimation("run animation");
                else
                    PlayAnimation("walk animation");
            }
            else
            {
                if (isRunning)
                    PlayAnimation("run animation flipped");
                else
                    PlayAnimation("walk animation flipped");
            }
            return;
        }

        // No horizontal movement: always play idle (choose left/right based on last direction)
        if (direction < 0)
            PlayAnimation("idle animation left");
        else
            PlayAnimation("idle animation right");
    }

    private void PlayAnimation(string animName)
    {
        if (currentAnimation == animName) return;

        animator.Play(animName);
        currentAnimation = animName;
    }
}
