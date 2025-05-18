using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    public PlayerHealth playerHealth;

    public Transform playerPos;

    public Vector2 playerDistance;
    public float xDistance;
    public float playerDistanceMin = 1;
    public float playerDistanceOpt = 3;
    public float playerDistanceMax = 8;

    public bool playerDetected;
    public bool hasAttacked;

    public float speed = 0.1f;

    private string currentAnimation;
    public Animator animator;

    private float timer = 0f;

    private float defenseChance = 50;

    private enum State { none, offensive, defensive };
    private State state = State.none;


    #region Logic
    void Update()
    {
        playerDistance = playerPos.position - transform.position;
        xDistance = Mathf.Abs(playerDistance.x);

        int duration = Random.Range(3, 7);
        UpdateState(duration); // every 5 seconds
        switch (state)
        {
            case State.defensive:
                Defensive();
                break;
            case State.offensive:
                Offensive();
                break;
        }
    }
    #endregion


    #region Random
    private void UpdateState(int duration)
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            timer = 0f;
            NewState();
            Debug.Log("State: " + state);
        }
    }

    private void NewState()
    {
        // Generate random value between 0 and 100
        float randomValue = Random.Range(0, 101);
        if (randomValue <= defenseChance)
        {
            state = State.defensive;
            defenseChance -= 20;
        }
        else
        {
            state = State.offensive;
            defenseChance += 20;
        }
    }
    #endregion


    #region Defensive
    private void Defensive()
    {
        if (xDistance < playerDistanceMin && !Player.isAttacking)
        {
            Attack();
            WalkAway();
        }
        else
        {
            hasAttacked = false;
            PlayAnimation("idle animation right");
        }
    }

    private void WalkAway()
    {
        if (playerDistance.x > 0)
        {
            // Player is to the right, so walk left (away)
            PlayAnimation("walk animation");
            transform.position -= new Vector3(speed, 0, 0);
        }
        else if (playerDistance.x < 0)
        {
            // Player is to the left, so walk right (away)
            PlayAnimation("walk animation flipped");
            transform.position += new Vector3(speed, 0, 0);
        }
        else
        {
            // Player is directly above/below, pick a direction (default: right)
            PlayAnimation("walk animation flipped");
            transform.position += new Vector3(speed, 0, 0);
        }
    }
    #endregion


    #region Offensive
    private void Offensive()
    {
        if (xDistance < playerDistanceMax && !Player.isAttacking)
        {
            WalkToward();
        }
        else PlayAnimation("idle animation right");
    }

    private void WalkToward()
    {
        if (playerDistance.x > playerDistanceOpt && !Player.isAttacking)
        {
            // Player is to the right, so walk right (toward)
            PlayAnimation("walk animation");
            transform.position += new Vector3(speed, 0, 0);
        }
        else if (playerDistance.x < -playerDistanceOpt && !Player.isAttacking)
        {
            // Player is to the left, so walk left (toward)
            PlayAnimation("walk animation flipped");
            transform.position -= new Vector3(speed, 0, 0);
        }
        else
        {
            PlayAnimation("idle animation right");
        }
    }
    #endregion


    #region Other methods
    private void Attack()
    {
        if (!hasAttacked)
        {
            PlayAnimation("knight sword attack");
            playerHealth.TakeDamage(20, transform.position);
            hasAttacked = true;
        }
        else return;
    }

    private void PlayAnimation(string animName)
    {
        if (currentAnimation == animName) return;

        animator.Play(animName);
        currentAnimation = animName;
    }
    #endregion
}
