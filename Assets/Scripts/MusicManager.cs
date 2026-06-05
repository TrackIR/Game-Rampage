using UnityEngine;

[System.Serializable]
public class Song
{
    public AudioClip intro;
    public AudioClip loop;
}

public class MusicManager : MonoBehaviour
{
    [Range(0f, 1f)]
    public float volume = 1f;

    public Song[] songs;

    private AudioSource introSource;
    private AudioSource loopSource;

    private bool isPlaying = false;

    void Start()
    {
        introSource = gameObject.AddComponent<AudioSource>();
        loopSource = gameObject.AddComponent<AudioSource>();

        introSource.volume = volume;
        loopSource.volume = volume;

        introSource.playOnAwake = false;
        loopSource.playOnAwake = false;
    }

    void Update()
    {
        if (isPlaying) return;

        Song randSong = songs[Random.Range(0, songs.Length)];
        PlaySong(randSong);
    }

    void PlaySong(Song song)
    {
        isPlaying = true;

        introSource.clip = song.intro;

        loopSource.clip = song.loop;
        loopSource.loop = true;

        double startTime = AudioSettings.dspTime;

        // play intro
        introSource.PlayScheduled(startTime);

        // schedule loop exactly when intro ends
        double loopStartTime = startTime + song.intro.length + 0.1;
        loopSource.PlayScheduled(loopStartTime);
    }
}