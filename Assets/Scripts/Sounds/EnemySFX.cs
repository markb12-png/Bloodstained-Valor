using UnityEngine;

public class EnemySFX : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip enemyFootsteps;
    public AudioClip enemyDash;
    public AudioClip enemyHurt;
    public AudioClip enemyAnticipation1;
    public AudioClip enemyAnticipation2;
    public AudioClip enemySweepAlert;
    public AudioClip enemySwordSwing1;
    public AudioClip enemySwordSwing2;
    public AudioClip enemySwordSwing3;
    public AudioClip enemyAttackAlert;
    public AudioClip enemyDazed;

    public void PlayFootsteps()
    {
        audioSource.PlayOneShot(enemyFootsteps);
    }

    public void PlayDash()
    {
        audioSource.PlayOneShot(enemyDash);
    }

    public void PlayHurt()
    {
        audioSource.PlayOneShot(enemyHurt);
    }

    public void PlayAnticipation1()
    {
        audioSource.PlayOneShot(enemyAnticipation1);
    }

    public void PlayAnticipation2()
    {
        audioSource.PlayOneShot(enemyAnticipation2);
    }

    public void PlaySweepAlert()
    {
        audioSource.PlayOneShot(enemySweepAlert);
    }

    public void PlaySwordSwing1()
    {
        audioSource.PlayOneShot(enemySwordSwing1);
    }

    public void PlaySwordSwing2()
    {
        audioSource.PlayOneShot(enemySwordSwing2);
    }

    public void PlaySwordSwing3()
    {
        audioSource.PlayOneShot(enemySwordSwing3);
    }

    public void PlayAttackAlert()
    {
        audioSource.PlayOneShot(enemyAttackAlert);
    }

    public void PlayDazed()
    {
        audioSource.PlayOneShot(enemyDazed);
    }
}
