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
        if (audioSource == null || punchClip == null) return;
        PlayOneShot(punchClip);
    }

    public void PlayDeath()
    {
        if (audioSource == null || deathClip == null) return;

        // Find all EnemyAudio instances and mute them
        EnemyAudio[] enemies = FindObjectsByType<EnemyAudio>(FindObjectsSortMode.None);
        foreach (EnemyAudio enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.MuteMe();
            }
        }

        audioSource.ignoreListenerPause = true;
        audioSource.PlayOneShot(deathClip);
    }

    public void PlayUltimate()
    {
        if (audioSource == null || ultimateClip == null) return;
        PlayOneShot(ultimateClip);
    }

    public void PlayHurt()
    {
        if (audioSource == null || hurtClip == null) return;
        PlayOneShot(hurtClip);
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}