using UnityEngine;

public class EnvironmentSFX : MonoBehaviour
{
    [Header("Ambient Settings")]
    public AudioSource ambianceSource;
    public AudioClip ambianceClip;
    public bool loop = true;
    public bool playOnStart = true;

    void Start()
    {
        if (ambianceSource == null)
        {
            ambianceSource = gameObject.AddComponent<AudioSource>();
        }

        ambianceSource.clip = ambianceClip;
        ambianceSource.loop = loop;

        if (playOnStart && ambianceClip != null)
        {
            ambianceSource.Play();
        }
    }

    // Optional: Call this from other scripts or events
    public void PlayAmbiance()
    {
        if (!ambianceSource.isPlaying && ambianceClip != null)
        {
            ambianceSource.Play();
        }
    }

    public void StopAmbiance()
    {
        if (ambianceSource.isPlaying)
        {
            ambianceSource.Stop();
        }
    }
}
