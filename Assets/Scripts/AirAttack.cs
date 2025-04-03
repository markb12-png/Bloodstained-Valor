using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirAttack : MonoBehaviour
{
    [Header("Hitbox Settings")]
    public GameObject hitboxPrefab; // Prefab for the attack hitbox
    public Vector2 hitboxOffset = new Vector2(0f, -1f); // Offset for the hitbox (downwards)
    public float hitboxDuration = 0.3f; // Duration for which the hitbox remains active

    [Header("Movement Settings")]
    public float downwardForce = -5f; // Downward force applied during the attack
    public float initialBoostUpward = 2f; // Slight upward boost before descending

    [Header("Attack Settings")]
    public int startupFrames = 16; // Increased startup delay before the attack (in frames)
    public int cooldownFrames = 17; // Additional immobilization after the attack (in frames)

    [Header("References")]
    private Rigidbody2D rb; // Player's Rigidbody2D component
    private GroundCheck groundCheck; // Reference to GroundCheck script
    private List<MonoBehaviour> scriptsToDisable; // List to manage other scripts on the player

    [Header("State")]
    private bool isAttacking = false; // Prevents multiple attacks at once

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<GroundCheck>();

        // Collect all MonoBehaviour scripts except this one
        scriptsToDisable = new List<MonoBehaviour>(GetComponents<MonoBehaviour>());
        scriptsToDisable.Remove(this);
    }

    void Update()
    {
        // Prevent input during an ongoing attack
        if (isAttacking)
        {
            return;
        }

        // Trigger the air attack when airborne and right mouse button is pressed
        if (Input.GetMouseButtonDown(1) && !groundCheck.IsGrounded())
        {
            StartCoroutine(AirAttackSequence());
        }
    }

    private IEnumerator AirAttackSequence()
    {
        isAttacking = true; // Prevent further input during the attack
        DisableAllScripts(); // Disable other scripts temporarily

        // Step 1: Stall the player for the startup frames (prep time)
        yield return new WaitForSeconds(startupFrames / 30f);

        // Step 2: Apply slight upward boost before the downward attack
        rb.velocity = new Vector2(rb.velocity.x, initialBoostUpward);
        yield return null; // Wait for 1 frame to apply the upward boost

        // Step 3: Apply downward force for the attack
        rb.velocity = new Vector2(rb.velocity.x, downwardForce);

        // Step 4: Spawn the hitbox
        Vector2 adjustedHitboxOffset = new Vector2(transform.position.x, transform.position.y + hitboxOffset.y);
        GameObject hitbox = Instantiate(hitboxPrefab, adjustedHitboxOffset, Quaternion.identity);

        // Destroy the hitbox after its duration
        Destroy(hitbox, hitboxDuration);

        // Step 5: Stall the player for the cooldown frames
        yield return new WaitForSeconds(cooldownFrames / 30f);

        // Step 6: Re-enable other scripts and end the attack sequence
        EnableAllScripts();
        isAttacking = false;
    }

    private void DisableAllScripts()
    {
        foreach (var script in scriptsToDisable)
        {
            script.enabled = false;
        }
    }

    private void EnableAllScripts()
    {
        foreach (var script in scriptsToDisable)
        {
            script.enabled = true;
        }
    }
}
