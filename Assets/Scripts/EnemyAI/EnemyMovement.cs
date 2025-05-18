using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform playerPos;

    public Vector2 playerDistance;
    public float playerDistanceMin = 1;
    public float playerDistanceOpt = 3;
    public float playerDistanceMax = 8;

    public bool playerDetected;

    public float speed = 0.1f;

    private string currentAnimation;
    public Animator animator;

    void Update()
    {
        playerDistance = (playerPos.position - transform.position);
        float xDistance = Mathf.Abs(playerDistance.x);

        if (xDistance < playerDistanceMin && !Player.isAttacking)
        {
            WalkAway();
        }    
        else PlayAnimation("idle animation right");
    }

    private void WalkAway()
    {
        if (playerDistance.x > 0)
        {
            PlayAnimation("walk animation");
            transform.position -= new Vector3(speed, 0, 0);
        }
        else
        {
            PlayAnimation("walk animation flipped");
            transform.position += new Vector3(speed, 0, 0);
        }
    }
        private void PlayAnimation(string animName)
    {
        if (currentAnimation == animName) return;

        animator.Play(animName);
        currentAnimation = animName;
    }
}
