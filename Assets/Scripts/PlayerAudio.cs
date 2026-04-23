using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Single Audio Source")]
    public AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip punchClip;
    public AudioClip deathClip;
    public AudioClip ultimateClip;
    public AudioClip hurtClip;

    public void PlayPunch()
    {
        PlayOneShot(punchClip);
    }

    public void PlayDeath()
    {
        PlayOneShot(deathClip);
    }

    public void PlayUltimate()
    {
        PlayOneShot(ultimateClip);
    }

    public void PlayHurt()
    {
        PlayOneShot(hurtClip);
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}