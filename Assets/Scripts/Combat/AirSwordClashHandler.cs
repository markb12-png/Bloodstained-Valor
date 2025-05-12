using System.Collections;
using System.Linq;
using UnityEngine;

public class AirSwordClashHandler : MonoBehaviour
{
    private HitboxDamage hitbox;

    void Awake() => hitbox = GetComponent<HitboxDamage>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        var otherClash = other.GetComponent<AirSwordClashHandler>();
        if (otherClash == null || otherClash == this) return;

        var ownerA = hitbox?.Owner;
        var ownerB = otherClash.hitbox?.Owner;

        if (ownerA == null || ownerB == null) return;

        if (ownerA.GetComponent<GroundDetector>()?.IsGrounded ?? true) return;
        if (ownerB.GetComponent<GroundDetector>()?.IsGrounded ?? true) return;

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
        caster.GetComponent<PlayerAirAttack>()?.InterruptBySwordClash();

        var toDisable = caster.GetComponents<MonoBehaviour>()
            .Where(s => !(s is GroundDetector || s is PlayerHealth || s is AirSwordClashHandler)).ToList();
        foreach (var s in toDisable) s.enabled = false;

        rb.velocity = Vector2.zero;
        for (int i = 0; i < 4; i++) yield return null;

        health?.TakeDamage(10f);

        float dir = Mathf.Sign(caster.transform.position.x - opponent.transform.position.x);
        Vector2 arc = new Vector2(4f * dir, 10f);
        for (int i = 0; i < 20; i++) { rb.velocity = arc; yield return null; }

        rb.velocity = Vector2.zero;
        for (int i = 0; i < 11; i++) yield return null;

        EnableAfterClash(caster);
    }

    private void EnableAfterClash(GameObject obj)
    {
        EnableIfExists<PlayerAirAttack>(obj);
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
