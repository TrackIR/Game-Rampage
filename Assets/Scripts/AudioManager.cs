using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance; // Only one audio manager should exist across the entire game
    
    public AudioSource audioSource; // Audio Source attached to the audio manager

    [Range(0.0f, 1.0f)]
    public float volume = 1.0f;

    [Header("Audio Clips")]
    public AudioClip buildingDestroy;
    public AudioClip playerHurt;
    public AudioClip playerHeal;
    public AudioClip enemyHurt;
    public AudioClip enemyShoot;
    public AudioClip gainScore;

    

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
    if(clip != null && audioSource != null) 
    {
        audioSource.PlayOneShot(clip, volume); // Could maybe be played locally? Something like 3D audio
    }
    else
        {
            print("no audio source!");
        }

    }
}
