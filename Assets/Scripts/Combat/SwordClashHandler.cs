using System.Collections;
using System.Linq;
using UnityEngine;

public class SwordClashHandler : MonoBehaviour
{
    private HitboxDamage hitbox;

    void Awake() => hitbox = GetComponent<HitboxDamage>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        var otherClash = other.GetComponent<SwordClashHandler>();
        if (otherClash == null || otherClash == this) return;

        var ownerA = hitbox?.Owner;
        var ownerB = otherClash.hitbox?.Owner;

        if (ownerA == null || ownerB == null) return;

        // Both must be on the ground
        if (!ownerA.GetComponent<GroundDetector>()?.IsGrounded ?? true) return;
        if (!ownerB.GetComponent<GroundDetector>()?.IsGrounded ?? true) return;

        hitbox.ClashTriggered = true;
        otherClash.hitbox.ClashTriggered = true;

        ownerA.GetComponent<MonoBehaviour>().StartCoroutine(ClashSequence(ownerA, ownerB));
        ownerB.GetComponent<MonoBehaviour>().StartCoroutine(ClashSequence(ownerB, ownerA));

        Destroy(gameObject);
        Destroy(other.gameObject);
    }

    private IEnumerator ClashSequence(GameObject caster, GameObject opponent)
    {
        var rb = caster.GetComponent<Rigidbody2D>();
        var health = caster.GetComponent<PlayerHealth>();

        caster.GetComponent<PlayerAttack>()?.InterruptBySwordClash();
        caster.GetComponent<PlayerHalberdAttack>()?.InterruptBySwordClash();

        // Disable all non-essential scripts
        var toDisable = caster.GetComponents<MonoBehaviour>()
            .Where(s => !(s is GroundDetector || s is PlayerHealth || s is SwordClashHandler)).ToList();
        foreach (var s in toDisable) s.enabled = false;

        rb.velocity = Vector2.zero;
        for (int i = 0; i < 4; i++) yield return null;

        // ✅ Apply knockback-aware damage
        health?.TakeDamage(10f, opponent.transform.position);

        // Brief knockback motion (5 frames)
        float dir = Mathf.Sign(caster.transform.position.x - opponent.transform.position.x);
        Vector2 knockback = new Vector2(8f * dir, 0);
        for (int i = 0; i < 5; i++) { rb.velocity = knockback; yield return null; }

        rb.velocity = Vector2.zero;
        for (int i = 0; i < 26; i++) yield return null;

        EnableAfterClash(caster);
    }

    private void EnableAfterClash(GameObject obj)
    {
        EnableIfExists<PlayerAttack>(obj);
        EnableIfExists<PlayerHalberdAttack>(obj);
        EnableIfExists<PlayerMovement>(obj);
        EnableIfExists<PlayerJump>(obj);
        EnableIfExists<PlayerFacing>(obj);
        EnableIfExists<PlayerDash>(obj);
        EnableIfExists<PlayerHealth>(obj);
        EnableIfExists<GroundDetector>(obj);
    }

    private void EnableIfExists<T>(GameObject obj) where T : MonoBehaviour
    {
        var comp = obj.GetComponent<T>();
        if (comp != null) comp.enabled = true;
    }
}
