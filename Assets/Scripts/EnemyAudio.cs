using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Audio Clips")]
    [SerializeField, Range(0f, 2f)]
    public float pitchRange = 0; // zero being no pitch randomizing
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
        float randomPitch = 1f + Random.Range(-pitchRange, pitchRange);
        audioSource.pitch = randomPitch;

        PlayOneShot(forwardClip);

        audioSource.pitch = 1; // set back to normal
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