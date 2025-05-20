using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private Slider healthSlider;

    [Header("Stun Settings")]
    [SerializeField] private float stunDuration = 0.5f;
    [SerializeField] private float knockbackForce = 10f;

    [Header("Deathblow Settings")]
    [SerializeField] private Color deathblowColor = new Color(0.8f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color normalHealthColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    [SerializeField] private GameObject enemyVisual;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float deathblowDuration = 1.2f;

    private float currentHealth;
    public bool isDead = false;
    public bool isInDeathblowState = false;
    public bool closeToPlayer = false;

    private MonoBehaviour[] enemyScripts;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool deathblowTriggered = false;

    private GameObject player;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;

        enemyScripts = GetComponents<MonoBehaviour>().Where(script => script != this).ToArray();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (isInDeathblowState && !isDead && closeToPlayer && Input.GetButtonDown("Attack") && !deathblowTriggered)
        {
            StartCoroutine(PlayDeathblowSequence());
        }
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Player")) closeToPlayer = true;
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player")) closeToPlayer = false;
    }

    public void TakeDamage(float amount, Vector2 hitSourcePosition)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (rb != null)
        {
            float direction = Mathf.Sign(transform.position.x - hitSourcePosition.x);
            if (closeToPlayer) direction = -direction;
            Vector2 knockbackDir = new Vector2(direction, 0.3f).normalized;
            rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        }

        if (currentHealth > 0)
        {
            StartCoroutine(StunRoutine());
        }
        else if (currentHealth <= 0 && !isInDeathblowState && !isDead)
        {
            EnterDeathblowState();
        }
    }

    private void EnterDeathblowState()
    {
        isInDeathblowState = true;

        if (spriteRenderer != null)
            spriteRenderer.color = deathblowColor;

        if (animator != null)
            animator.Play("enemy dazed");

        LockEnemy();
    }

    private IEnumerator StunRoutine()
    {
        animator.Play("enemy stun");

        foreach (var script in enemyScripts)
            script.enabled = false;

        yield return new WaitForSeconds(stunDuration);

        foreach (var script in enemyScripts)
            script.enabled = true;

        animator.Play("enemy idle");
    }

    private IEnumerator PlayDeathblowSequence()
    {
        deathblowTriggered = true;
        isDead = true;

        LockEnemy();

        // Disable specific player scripts
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

        if (enemyVisual != null)
            enemyVisual.SetActive(false);

        if (playerAnimator != null)
            playerAnimator.Play("knight deathblow");

        yield return new WaitForSeconds(deathblowDuration);

        if (animator != null)
            animator.Play("enemy dead");

        if (enemyVisual != null)
            enemyVisual.SetActive(true);

        // Re-enable same scripts
        if (movement) movement.enabled = true;
        if (jump) jump.enabled = true;
        if (dash) dash.enabled = true;
        if (attack) attack.enabled = true;
        if (airAttack) airAttack.enabled = true;
        if (facing) facing.enabled = true;
    }

    private IEnumerator HandleDeath()
    {
        isDead = true;
        LockEnemy();

        foreach (var script in enemyScripts)
        {
            if (script != null)
                script.enabled = false;
        }

        if (animator != null)
            animator.Play("enemy dead");

        if (healthSlider != null)
            healthSlider.gameObject.SetActive(false);

        yield return null;
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
            if (healthSlider.fillRect != null)
            {
                Image fillImage = healthSlider.fillRect.GetComponent<Image>();
                if (fillImage != null)
                    fillImage.color = isInDeathblowState ? deathblowColor : normalHealthColor;
            }
        }
    }
}
