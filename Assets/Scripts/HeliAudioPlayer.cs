using UnityEngine;

public class HeliAudioPlayer : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource bladesSource;
    public AudioSource deathSource;
    public AudioSource waterJetSource;
    public AudioSource hurtSource;

    public void PlayWaterJet(bool play)
    {
        if (waterJetSource == null) return;

        if (play)
        {
            if (!waterJetSource.isPlaying)
            {
                waterJetSource.loop = true;
                waterJetSource.Play();
            }
        }
        else
        {
            waterJetSource.Stop();
        }
    }

    public void PlayBlades()
    {
        if (bladesSource != null)
            bladesSource.Play();
    }

    public void PlayHurt()
    {
        if (hurtSource != null)
            hurtSource.Play();
    }

    public void PlayDeath()
    {
        if (deathSource != null)
            deathSource.Play();
    }
}
