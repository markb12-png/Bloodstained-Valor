using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Linq;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private Slider healthSlider;

    [Header("Stun Settings")]
    [SerializeField] private float stunDuration = 0.5f;

    [Header("Deathblow Settings")]
    [SerializeField] private Color deathblowColor = new Color(0.8f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color normalHealthColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    [SerializeField] private GameObject enemyVisual;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private GameObject deathblowUIObject;

    [Header("Attack Range Detector")]
    [SerializeField] private Collider2D attackRangeDetector;

    [Header("Fogwall Trigger Zone")]
    [SerializeField] private Collider2D fogwallTriggerCollider;

    [Header("Fogwalls")]
    [SerializeField] private GameObject fogwallToActivate;

    private float currentHealth;
    public bool isDead = false;
    public bool isInDeathblowState = false;
    public bool closeToPlayer = false;

    private MonoBehaviour[] enemyScripts;
    private EnemyBrain enemyBrain;
    private EnemyCombat enemyCombat;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool deathblowTriggered = false;

    private GameObject player;
    private GroundDetector playerGround;
    private bool hasTriggeredFogwall = false;
    private Coroutine attackCoroutine;

    EnemyCombat EnemyCombat;

    private void Start()
    {
        EnemyCombat = this.gameObject.GetComponent<EnemyCombat>();
        currentHealth = maxHealth;
        UpdateHealthUI();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;

        // Get specific enemy components
        enemyBrain = GetComponent<EnemyBrain>();
        enemyCombat = GetComponent<EnemyCombat>();

        // Get all other MonoBehaviour scripts except this one
        enemyScripts = GetComponents<MonoBehaviour>()
            .Where(script => script != this && script != enemyBrain && script != enemyCombat)
            .ToArray();

        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            playerGround = player.GetComponent<GroundDetector>();

        if (deathblowUIObject != null)
            deathblowUIObject.SetActive(false);
    }

    private void Update()
    {
        if (!hasTriggeredFogwall && !isInDeathblowState && player != null && fogwallTriggerCollider != null && fogwallTriggerCollider.bounds.Intersects(player.GetComponent<Collider2D>().bounds))
        {
            if (fogwallToActivate != null)
            {
                fogwallToActivate.SetActive(true);
                hasTriggeredFogwall = true;
            }
        }

        if (attackRangeDetector != null && player != null)
        {
            closeToPlayer = attackRangeDetector.bounds.Intersects(player.GetComponent<Collider2D>().bounds);
        }

        if (isInDeathblowState && !isDead && closeToPlayer && Input.GetButtonDown("Attack") && !deathblowTriggered)
        {
            if (playerGround != null && playerGround.IsGrounded)
            {
                StartCoroutine(PlayDeathblowSequence());
            }
        }
    }

    public void TakeDamage(float amount, Vector2 hitSourcePosition)
    {
        if (rb != null && player != null)
        {
            Vector2 knockbackDir = (transform.position - player.transform.position).normalized;
            rb.AddForce(knockbackDir * 2f, ForceMode2D.Impulse);
        }
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth > 0)
        {
            StartCoroutine(StunRoutine());
        }
        else if (!isInDeathblowState && !isDead)
        {
            StartCoroutine(DelayedDeathblowEntry());
        }
    }

    private IEnumerator DelayedDeathblowEntry()
    {
        yield return new WaitForSeconds(0.3f);
        EnterDeathblowState();
    }

    private void EnterDeathblowState()
    {
        // Destroy enemy behavior scripts
        Destroy(GetComponent<EnemyMovement>());
        Destroy(enemyBrain);
        Destroy(enemyCombat);

        animator.Play("enemy dazed", -1, 0f); // Play from the beginning
        if (deathblowUIObject != null)
            deathblowUIObject.SetActive(true);

        isInDeathblowState = true;

        // Force the dazed animation to play immediately
        if (animator != null)
        {
            animator.StopPlayback(); // Stop any current animation
            animator.Play("enemy dazed", -1, 0f); // Play from the beginning
            animator.Update(0f); // Force immediate update
        }

        if (spriteRenderer != null)
            spriteRenderer.color = deathblowColor;

        LockEnemy();

        // Disable all other scripts
        foreach (var script in enemyScripts)
            script.enabled = false;
    }


    private IEnumerator StunRoutine()
    {
        // Stop the attack coroutine if it's running
        StopCoroutine(EnemyCombat.EnemyAttack());
     

        if (isInDeathblowState) yield break;

        if (animator != null)
            animator.Play("enemy stun");

        // Disable specific enemy components first
        if (enemyCombat != null) enemyCombat.enabled = false;
        if (enemyBrain != null) enemyBrain.enabled = false;

        // Then disable all other scripts
        foreach (var script in enemyScripts)
            script.enabled = false;

        yield return new WaitForSeconds(stunDuration);

        if (!isInDeathblowState)
        {
            // Re-enable all scripts
            foreach (var script in enemyScripts)
                script.enabled = true;

            if (enemyBrain != null) enemyBrain.enabled = true;
            if (enemyCombat != null) enemyCombat.enabled = true;

            if (animator != null)
            {
                if (player != null && player.GetComponent<Rigidbody2D>().velocity.x < 0)
                    animator.Play("enemy idle left");
                else
                    animator.Play("enemy idle right");
            }
        }
    }

    private IEnumerator PlayDeathblowSequence()
    {
        if (deathblowUIObject != null)
            deathblowUIObject.SetActive(false);

        deathblowTriggered = true;
        isDead = true;
        gameObject.layer = LayerMask.NameToLayer("Dead");
        LockEnemy();

        if (healthSlider != null)
        {
            Destroy(healthSlider.gameObject);
            healthSlider = null;
        }

        if (animator != null)
            animator.Play("enemy dead");

        var movement = player.GetComponent<PlayerMovement>();
        var jump = player.GetComponent<PlayerJump>();
        var dash = player.GetComponent<PlayerDash>();
        var attack = player.GetComponent<PlayerAttack>();
        var airAttack = player.GetComponent<PlayerAirAttack>();
        var facing = player.GetComponent<PlayerFacing>();

        if (movement) movement.enabled = false;
        if (jump) jump.enabled = false;
        if (dash) dash.enabled = false;
        if (attack) attack.enabled = false;
        if (airAttack) airAttack.enabled = false;
        if (facing) facing.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (playerAnimator != null)
            playerAnimator.Play("knight deathblow");

        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo info = playerAnimator.GetCurrentAnimatorStateInfo(0);
            return info.IsName("knight deathblow") && info.normalizedTime >= 1f;
        });

        if (playerAnimator != null)
        {
            if (player.transform.localScale.x > 0)
                playerAnimator.Play("idle animation right");
            else
                playerAnimator.Play("idle animation left");
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        transform.position += new Vector3(2f, 0f, 0f);
        if (spriteRenderer != null)
            spriteRenderer.flipX = !spriteRenderer.flipX;

        if (movement) movement.enabled = true;
        if (jump) jump.enabled = true;
        if (dash) dash.enabled = true;
        if (attack) attack.enabled = true;
        if (airAttack) airAttack.enabled = true;
        if (facing) facing.enabled = true;

        if (animator != null)
        {
            if (player != null && player.GetComponent<Rigidbody2D>().velocity.x < 0)
                animator.Play("enemy idle left");
            else
                animator.Play("enemy idle right");
        }

        if (fogwallToActivate != null)
        {
            fogwallToActivate.SetActive(false);
        }
    }

    public void LockEnemy()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }
}