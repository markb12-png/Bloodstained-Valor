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
        HandleAnimation();

        // Use Input Manager for sprint
        if (Input.GetButton("Sprint"))
        {
            speed = runSpeed;
        }
        else
        {
            speed = walkSpeed;
        }
    }

    private void HandleMovement()
    {
        // Use horizontal axis from Input Manager (supports keyboard + controller)
        float move = Input.GetAxisRaw("Horizontal"); // Returns -1, 0, or 1
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
    }

    private void HandleAnimation()
    {
        float horizontalSpeed = Mathf.Abs(rb.velocity.x);
        float direction = Mathf.Sign(rb.velocity.x);

        if (horizontalSpeed > 0f)
        {
            if (Mathf.Approximately(horizontalSpeed, walkSpeed))
            {
                Player.state = Player.Move.isWalking;
                if (direction > 0)
                    PlayAnimation("walk animation");
                else
                    PlayAnimation("walk animation flipped");
            }
            else if (Mathf.Approximately(horizontalSpeed, runSpeed))
            {
                Player.state = Player.Move.isRunning;
                if (direction > 0)
                    PlayAnimation("run animation");
                else
                    PlayAnimation("run animations flipped");
            }

            return;
        }

        PlayAnimation("idle animation right");
    }

    private void PlayAnimation(string animName)
    {
        if (currentAnimation == animName) return;

        animator.Play(animName);
        currentAnimation = animName;
    }
}
