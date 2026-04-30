using UnityEngine;

public class DestroyAfterAudio : MonoBehaviour
{
    public AudioSource audioSource;
    private float soundClipLength;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (audioSource != null)
        {
            soundClipLength = audioSource.clip.length;
        } else
        {
            soundClipLength = 1f;
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
