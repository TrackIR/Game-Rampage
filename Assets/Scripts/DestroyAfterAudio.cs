using UnityEngine;

public class DestroyAfterAudio : MonoBehaviour
{
    public AudioSource audioSource;
    
    [SerializeField, Range(0f, 2f)]
    public float pitchRange = 0; // zero being no pitch randomizing
    private float soundClipLength;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (audioSource != null)
        {
            soundClipLength = audioSource.clip.length;
            
            float randomPitch = 1f + Random.Range(-pitchRange, pitchRange);
            audioSource.pitch = randomPitch;

            audioSource.Play();
        } else
        {
            soundClipLength = 0.1f;
        }
    }

    private float timeElapsed = 0f;
    void Update()
    {
        if (timeElapsed > soundClipLength)
        {
            Destroy(gameObject);
        }
        timeElapsed += Time.deltaTime;
    }
}
