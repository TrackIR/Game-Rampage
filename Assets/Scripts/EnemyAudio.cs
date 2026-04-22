using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip deathSound;
    public AudioClip waterJetSound;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayWaterJet(bool play)
    {
        if (play)
        {
            audioSource.clip = waterJetSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
            audioSource.loop = false;
        }
    }

    public void PlayDeath()
    {
        audioSource.PlayOneShot(deathSound);
    }
}