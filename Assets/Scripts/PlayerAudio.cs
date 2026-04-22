using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource punchSource;
    public AudioSource deathSource;
    public AudioSource ultimateSource;
    public AudioSource hurtSource;

    public void PlayPunch()
    {
        if (punchSource != null)
            punchSource.Play();
    }

    public void PlayDeath()
    {
        if (deathSource != null)
            deathSource.Play();
    }

    public void PlayUltimate()
    {
        if (ultimateSource != null)
            ultimateSource.Play();
    }

    public void PlayHurt()
    {
        if (hurtSource != null)
            hurtSource.Play();
    }
}