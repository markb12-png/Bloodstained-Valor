using UnityEngine;

public class DeathUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Animator deathTextAnimator;   // Animator for "You Died" or similar text
    [SerializeField] private Animator screenFadeAnimator;  // Animator for screen darken

    public void TriggerDeathUI()
    {
        if (deathTextAnimator != null)
            deathTextAnimator.SetTrigger("Show");

        if (screenFadeAnimator != null)
            screenFadeAnimator.SetTrigger("FadeIn");

        Debug.Log("[UI] Death UI triggered.");
    }
}
