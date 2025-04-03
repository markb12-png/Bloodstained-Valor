using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject hitboxPrefab;
    public Vector2 hitboxOffset = new Vector2(1f, 0f);
    public float hitboxDuration = 0.3f;
    public int startupFrames = 10;
    public int cooldownFrames = 15;
    public float dashDistance = 2f;
    public float dashDuration = 0.2f;

    [Header("References")]
    private Rigidbody2D rb;
    private List<MonoBehaviour> allScripts;

    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        allScripts = new List<MonoBehaviour>(GetComponents<MonoBehaviour>());
        allScripts.Remove(this);
        allScripts.RemoveAll(script => script == null); // Remove null entries

        Debug.Log("PlayerAttack initialized. Managed scripts:");
        foreach (var script in allScripts)
        {
            Debug.Log(" - " + script.GetType().Name);
        }
    }

    void Update()
    {
        // Block attack logic if a clash is active
        if (PriorityManager.isClashActive)
        {
            Debug.Log("PlayerAttack blocked: SwordClash is active.");
            return; // Prevent PlayerAttack behavior while clash is active
        }

        if (isAttacking) return; // Prevent concurrent attack sequences

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("PlayerAttack input detected! Starting attack sequence.");
            StartCoroutine(AttackSequence());
        }
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        GlobalStateManager.isInteractionActive = true; // Set interaction as active

        Debug.Log("PlayerAttack sequence started. Disabling other scripts.");
        DisableAllScripts();

        // Startup delay
        yield return new WaitForSeconds(startupFrames / 30f);
        Debug.Log("Startup completed.");

        // Perform dash
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Debug.Log("Dashing in direction: " + direction);
        yield return StartCoroutine(Dash(direction));

        // Spawn hitbox
        Vector2 offset = transform.localScale.x > 0 ? hitboxOffset : new Vector2(-hitboxOffset.x, hitboxOffset.y);
        Debug.Log("Spawning hitbox at offset: " + offset);
        GameObject hitbox = Instantiate(hitboxPrefab, (Vector2)transform.position + offset, Quaternion.identity);
        Destroy(hitbox, hitboxDuration);
        Debug.Log("Hitbox spawned and will be destroyed after " + hitboxDuration + " seconds.");

        // Cooldown delay
        yield return new WaitForSeconds(cooldownFrames / 30f);
        Debug.Log("Cooldown completed.");

        EnableAllScripts();
        GlobalStateManager.isInteractionActive = false; // Clear interaction state
        isAttacking = false;
        Debug.Log("PlayerAttack sequence finished. Re-enabled other scripts.");
    }

    private IEnumerator Dash(Vector2 direction)
    {
        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            float speed = Mathf.Lerp(dashDistance, 0, elapsedTime / dashDuration);
            rb.velocity = direction * speed;
            Debug.Log("Dashing... Elapsed time: " + elapsedTime + ", Speed: " + speed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        Debug.Log("Dash completed.");
    }

    private void DisableAllScripts()
    {
        foreach (var script in allScripts)
        {
            if (script != null)
            {
                script.enabled = false;
                Debug.Log("Disabled script: " + script.GetType().Name);
            }
        }
    }

    private void EnableAllScripts()
    {
        foreach (var script in allScripts)
        {
            if (script != null)
            {
                script.enabled = true;
                Debug.Log("Enabled script: " + script.GetType().Name);
            }
        }
    }
}
