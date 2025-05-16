using System.Collections;
using System.Linq;
using UnityEngine;

public class SwordClashHandler : MonoBehaviour
{
    private HitboxDamage hitbox;

    void Awake() => hitbox = GetComponent<HitboxDamage>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[SwordClash][Trigger] {gameObject.name} triggered with {other.gameObject.name} at frame {Time.frameCount}");

        var otherClash = other.GetComponent<SwordClashHandler>();
        if (otherClash == null || otherClash == this) return;

        var ownerA = hitbox?.Owner;
        var ownerB = otherClash.hitbox?.Owner;
        if (ownerA == null || ownerB == null) return;

        Debug.Log($"[SwordClash][CLASH] {gameObject.name} (owner: {ownerA.name}) vs {other.gameObject.name} (owner: {ownerB.name}) at frame {Time.frameCount}");

        // Disable all non-core scripts on both owners (not on the hitboxes!)
        DisableNonCoreScripts(ownerA);
        DisableNonCoreScripts(ownerB);

        // Start the clash sequence for both casters using a reliable MonoBehaviour
        var ownerAScript = ownerA.GetComponents<MonoBehaviour>().FirstOrDefault(m => m != null && m.enabled);
        var ownerBScript = ownerB.GetComponents<MonoBehaviour>().FirstOrDefault(m => m != null && m.enabled);

        if (ownerAScript != null)
            ownerAScript.StartCoroutine(ClashSequence(ownerA, ownerB));
        if (ownerBScript != null)
            ownerBScript.StartCoroutine(ClashSequence(ownerB, ownerA));

        Destroy(gameObject);
        Destroy(other.gameObject);
    }

    private void DisableNonCoreScripts(GameObject obj)
    {
        foreach (var script in obj.GetComponents<MonoBehaviour>())
        {
            if (script is PlayerHealth || script is EnemyHealth || script is GroundDetector || script is SwordClashHandler)
                continue;
            // Do not stop or disable on the hitbox, only on the player/enemy!
            script.enabled = false;
        }
    }

    private void EnableNonCoreScripts(GameObject obj)
    {
        foreach (var script in obj.GetComponents<MonoBehaviour>())
        {
            if (script is PlayerHealth || script is EnemyHealth || script is GroundDetector || script is SwordClashHandler)
                continue;
            script.enabled = true;
        }
    }

    private IEnumerator ClashSequence(GameObject caster, GameObject opponent)
    {
        var rb = caster.GetComponent<Rigidbody2D>();

        // Stall for 4 frames
        for (int i = 0; i < 4; i++) yield return null;

        // Apply 10 damage
        caster.GetComponent<PlayerHealth>()?.TakeDamage(10f, opponent.transform.position);
        caster.GetComponent<EnemyHealth>()?.TakeDamage(10f, opponent.transform.position);

        // Dash back for 5 frames
        if (rb != null)
        {
            float dashDir = Mathf.Sign(caster.transform.position.x - opponent.transform.position.x);
            Vector2 dash = new Vector2(8f * dashDir, 0);
            for (int i = 0; i < 5; i++)
            {
                rb.velocity = dash;
                yield return null;
            }
            rb.velocity = Vector2.zero;
        }

        // Stall for another 31 frames (total 36 frames)
        for (int i = 0; i < 31; i++) yield return null;

        // Re-enable all scripts (except core)
        EnableNonCoreScripts(caster);
        Debug.Log($"[SwordClash][END] {caster.name} restored after clash.");
    }
}
