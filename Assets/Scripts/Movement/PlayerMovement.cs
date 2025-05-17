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

        if (Input.GetKey(KeyCode.LeftShift)) {
            speed = runSpeed;
        }
        else speed = walkSpeed;
    }

    private void HandleMovement() {

        float move = 0f;
        if (Input.GetKey(KeyCode.A)) move -= 1f;
        if (Input.GetKey(KeyCode.D)) move += 1f;

        rb.velocity = new Vector2(move * speed, rb.velocity.y);
    }

    private void HandleAnimation() {

        float horizontalSpeed = Mathf.Abs(rb.velocity.x);
        float direction = Mathf.Sign(rb.velocity.x);

        if (horizontalSpeed > 0f) {
            if (Mathf.Approximately(horizontalSpeed, walkSpeed)) {
                Player.state = Player.Move.isWalking;
                if (direction > 0)
                    PlayAnimation("walk animation");
                else
                    PlayAnimation("walk animation flipped");
            }
            else if (Mathf.Approximately(horizontalSpeed, runSpeed)) {
                Player.state = Player.Move.isRunning;
                if (direction > 0)
                    PlayAnimation("run animation");
                else
                    PlayAnimation("run animations flipped");
            }

            return;
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
