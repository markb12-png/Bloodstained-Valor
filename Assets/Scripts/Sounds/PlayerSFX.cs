using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip footstepSound;
    public AudioClip swordSwing1;
    public AudioClip swordSwing2;
    public AudioClip dashsound;
    public AudioClip jumpstart;
    public AudioClip land;
    public AudioClip deathstrike;
    public AudioClip deathtumble;
    public AudioClip darksouls;
    public AudioClip hurt;

    public AudioClip deathblowBlood;
    public AudioClip deathblowInitialStab;
    public AudioClip deathblowAnticipation;
    public AudioClip deathblowWipe;
    public AudioClip deathblowSpin;
    public AudioClip deathblowSwordOut;
    public AudioClip deathblowIndicator; // New

    // Footstep sound for walk/run animations
    public void PlayFootstep()
    {
        audioSource.PlayOneShot(footstepSound);
    }

    // Sword swing sounds for attack animations
    public void PlaySwordSwing1()
    {
        audioSource.PlayOneShot(swordSwing1);
    }

    public void PlaySwordSwing2()
    {
        audioSource.PlayOneShot(swordSwing2);
    }

    public void PlayDash()
    {
        audioSource.PlayOneShot(dashsound);
    }

    public void PlayJump()
    {
        audioSource.PlayOneShot(jumpstart);
    }

    public void PlayLand()
    {
        audioSource.PlayOneShot(land);
    }

    public void Playdeathstrike()
    {
        audioSource.PlayOneShot(deathstrike);
    }

    public void Playdeathtumble()
    {
        audioSource.PlayOneShot(deathtumble);
    }

    public void Playdarksouls()
    {
        audioSource.PlayOneShot(darksouls);
    }

    public void PlayHurt()
    {
        audioSource.PlayOneShot(hurt);
    }

    public void PlayDeathblowBlood()
    {
        audioSource.PlayOneShot(deathblowBlood);
    }

    public void PlayDeathblowInitialStab()
    {
        audioSource.PlayOneShot(deathblowInitialStab);
    }

    public void PlayDeathblowAnticipation()
    {
        audioSource.PlayOneShot(deathblowAnticipation);
    }

    public void PlayDeathblowWipe()
    {
        audioSource.PlayOneShot(deathblowWipe);
    }

    public void PlayDeathblowSpin()
    {
        audioSource.PlayOneShot(deathblowSpin);
    }

    public void PlayDeathblowSwordOut()
    {
        audioSource.PlayOneShot(deathblowSwordOut);
    }

    public void PlayDeathblowIndicator() // New function
    {
        audioSource.PlayOneShot(deathblowIndicator);
    }
}
