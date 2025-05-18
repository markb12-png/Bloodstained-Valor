using UnityEngine;

public class UISFX : MonoBehaviour
{
    [Header("UI Sounds")]
    public AudioSource audioSource;
    public AudioClip dashRechargeClip;

    // Play this when the player's dash becomes available again
    public void PlayDashRecharge()
    {
      
            audioSource.PlayOneShot(dashRechargeClip);
    }
}
