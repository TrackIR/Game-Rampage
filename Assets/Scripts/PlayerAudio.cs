using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip punchSound;
    public AudioClip deathSound;
    public AudioClip ultimateSound;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayPunch()
    {
        audioSource.PlayOneShot(punchSound);
    }

    public void PlayDeath()
    {
        audioSource.PlayOneShot(deathSound);
    }

    public void PlayUltimate()
    {
        audioSource.PlayOneShot(ultimateSound);
    }
}