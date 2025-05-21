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
    [SerializeField] private float knockbackForce = 1.5f;

    [Header("Deathblow Settings")]
    [SerializeField] private Color deathblowColor = new Color(0.8f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color normalHealthColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    [SerializeField] private GameObject enemyVisual;
    [SerializeField] private Animator playerAnimator;

    [Header("Attack Collider")]
    [SerializeField] private BoxCollider2D attackCollider; // Assign this in Inspector

    [Header("Dazed UI")]
    [SerializeField] private GameObject dazedTextPrefab;
    [SerializeField] private Vector3 dazedTextOffset = new Vector3(0, 2f, 0);
    private GameObject activeDazedText;

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
    private GroundDetector playerGround;

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

        if (player != null)
            playerGround = player.GetComponent<GroundDetector>();
    }

    private void Update()
    {
        if (isInDeathblowState && activeDazedText != null)
        {
            activeDazedText.transform.position = transform.position + dazedTextOffset;
        }

        // ✅ Use overlap check against attack collider to see if player is inside
        if (attackCollider != null && player != null)
        {
            closeToPlayer = attackCollider.bounds.Contains(player.transform.position);
        }

        if (isInDeathblowState && !isDead && closeToPlayer && Input.GetButtonDown("Attack") && !deathblowTriggered)
        {
            if (playerGround != null && playerGround.IsGrounded)
            {
                StartCoroutine(PlayDeathblowSequence());
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && attackCollider.bounds.Intersects(collider.bounds))
        {
            closeToPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            closeToPlayer = false;
        }
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
            Vector2 knockbackDir = new Vector2(direction, 0.3f);
            rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        }

        if (currentHealth > 0)
        {
            StartCoroutine(StunRoutine());
        }
        else if (currentHealth <= 0 && !isInDeathblowState && !isDead)
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
        isInDeathblowState = true;

        if (animator != null)
            animator.Play("enemy dazed", -1, 0f);

        if (spriteRenderer != null)
            spriteRenderer.color = deathblowColor;

        LockEnemy();

        foreach (var script in enemyScripts)
            script.enabled = false;

        if (attackCollider != null)
            attackCollider.enabled = false; // Optional: disable attack collider during dazed/deathblow

        if (dazedTextPrefab != null && activeDazedText == null)
        {
            activeDazedText = Instantiate(dazedTextPrefab, transform.position + dazedTextOffset, Quaternion.identity);
        }
    }

    private IEnumerator StunRoutine()
    {
        if (isInDeathblowState) yield break;

        if (animator != null)
            animator.Play("enemy stun");

        foreach (var script in enemyScripts)
            script.enabled = false;

        yield return new WaitForSeconds(stunDuration);

        if (!isInDeathblowState)
        {
            foreach (var script in enemyScripts)
                script.enabled = true;

            if (animator != null)
                animator.Play("enemy idle");
        }
    }

    private IEnumerator PlayDeathblowSequence()
    {
        deathblowTriggered = true;
        isDead = true;

        LockEnemy();

        if (activeDazedText != null)
        {
            Destroy(activeDazedText);
            activeDazedText = null;
        }

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

        if (player.transform.localScale.x > 0)
            playerAnimator.Play("idle animation right");
        else
            playerAnimator.Play("idle animation left");

        // ✅ Deactivate fog walls
        GameObject.Find("Fogwall1")?.SetActive(false);
        GameObject.Find("Fogwall2")?.SetActive(false);
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
