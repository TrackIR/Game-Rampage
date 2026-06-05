using Unity.VisualScripting;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Single Audio Source")]
    public AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip punchClip;

    [SerializeField, Range(0f, 1f)]
    public float punchVol;
    public AudioClip deathClip;

    [SerializeField, Range(0f, 1f)]
    public float deathVol;
    public AudioClip ultimateClip;

    [SerializeField, Range(0f, 1f)]
    public float ultimateVol;
    public AudioClip hurtClip;

    [SerializeField, Range(0f, 1f)]
    public float hurtVol;

    public AudioClip shockwave;
    [SerializeField, Range(0f, 1f)]
    public float shockwaveVol;

    public AudioClip standUp;
    [SerializeField, Range(0f, 1f)]
    public float standUpVol;

    public void PlayPunch()
    {
        if (audioSource == null || punchClip == null) return;
        audioSource.volume = punchVol;
        PlayOneShot(punchClip);
    }

    public void PlayDeath()
    {
        if (audioSource == null || deathClip == null) return;

        audioSource.volume = deathVol;

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

        audioSource.volume = ultimateVol;

        PlayOneShot(ultimateClip);
    }

    public void PlayHurt()
    {
        if (audioSource == null || hurtClip == null) return;
        audioSource.volume = hurtVol;
        PlayOneShot(hurtClip);
    }

    public void PlayShockWave()
    {
        if (audioSource == null || shockwave == null) return;
        audioSource.volume = shockwaveVol;
        PlayOneShot(shockwave);
    }

    public void PlayStandUp()
    {
        if (audioSource == null || standUp == null) return;
        audioSource.volume = standUpVol;
        PlayOneShot(standUp);
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}