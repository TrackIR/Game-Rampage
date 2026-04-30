using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip forwardClip;
    public AudioClip deathClip;
    public AudioClip waterJetClip;
    public AudioClip hurtClip;

    public void MuteMe()
    {
        audioSource.mute = true;
    }

    public void PlayWaterJet(bool play)
    {
        if (audioSource == null || waterJetClip == null) return;

        audioSource.ignoreListenerPause = false;

        if (play)
        {
            if (audioSource.clip != waterJetClip || !audioSource.isPlaying)
            {
                audioSource.clip = waterJetClip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.clip == waterJetClip)
            {
                audioSource.Stop();
            }
        }
    }

    public void PlayForward()
    {
        PlayOneShot(forwardClip);
    }

    public void PlayHurt()
    {
        PlayOneShot(hurtClip);
    }

    public void PlayDeath()
    {
        PlayOneShot(deathClip);
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}