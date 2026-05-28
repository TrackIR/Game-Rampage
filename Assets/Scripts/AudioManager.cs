using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource audioSource;

    [Range(0.0f, 1.0f)]
    public float volume = 1.0f;

    [Header("Audio Clips")]
    public AudioClip buildingDestroy;
    public AudioClip playerHurt;
    public AudioClip playerHeal;
    public AudioClip enemyHurt;
    public AudioClip enemyShoot;
    public AudioClip gainScore;

    [Header("UI Audio Clips")]
    public AudioClip menuHover;
    public AudioClip menuClick;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void playAudio(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
        else
        {
            print("no audio source!");
        }
    }
}