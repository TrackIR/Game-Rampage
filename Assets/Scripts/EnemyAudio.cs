using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource loopingSource;

    [Header("Audio Clips")]
    public AudioClip deathSound;
    public AudioClip waterJetSound;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (loopingSource == null)
            loopingSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayWaterJet(bool play)
    {
        if (play)
        {
            if (!loopingSource.isPlaying)
            {
                loopingSource.clip = waterJetSound;
                loopingSource.loop = true;
                loopingSource.Play();
            }
        }
        else
        {
            loopingSource.Stop();
        }
    }

    public void PlayDeath()
    {
        audioSource.PlayOneShot(deathSound);
    }
}