using UnityEngine;
using TMPro; //Using this to update text

public class ManageUI : MonoBehaviour
{

    // Audio Sources and Settings
    [Header("Audio")]
    public AudioSource scoreAudio; // Listing them like this so only one header is displayed in the editor
    public AudioSource healthGainAudio, healthLostAudio;
    public AudioClip scoreAudioClip, healthGainAudioClip, healthLostAudioClip;

    [Range(0f, 1f)]
    public float volume = 1f; // Change the sound of the audio sources

    // Health Variables
    [Header("Health  Variables")]
    public TMP_Text healthObject;
    public RectTransform healthBarObject, healthBarObjectFill;
    public int maxHealth = 100;

    [HideInInspector] // Can be used by other scripts, but doesnt show up
    private int health = 100;

    // Score Variables
    [Header("Score Variables")]
    public TMP_Text scoreObject;
    [HideInInspector]
    public int score = 0;

    // Timer Variables
    [Header("Timer Variables")]
    public TMP_Text timerObject;
    [Range(1f, 10f)]
    public float difficulty; // Not yet implemented, potentially used to make enemies spawn quicker

    [HideInInspector]
    public float timer = 0f;

    private bool timerRunning = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartTimer();
        UpdateHealthBar();
        ChangeScore(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            timer += Time.deltaTime;
            UpdateTimerUI(); // Done through a function so the timer can be started, stopped, or reset elsewhere
        }
    }

    public void SetVolunm(float amount)
    {
        print("Setting the volume to " + amount + "\n");
        volume = amount;
    }


    public void ChangeHealth(int amount)
    {
        health += amount;

        if (health <= 0)
        {
            print("HEALTH AT 0. ACT ON IT HERE");
        }
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        if (amount > 0) // Play the sound for gaining health
        {
            healthGainAudio.volume = volume;
            healthGainAudio.PlayOneShot(healthGainAudioClip);
        }
        else if (amount < 0) // Play the sound for losing health
        {
            healthLostAudio.volume = volume;
            healthLostAudio.PlayOneShot(healthLostAudioClip);
        }

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {

        string healthString = health + "/" + maxHealth;

        healthObject.text = healthString;

        float healthPercent = (float)health / (float)maxHealth;

        Vector2 size = healthBarObjectFill.sizeDelta;
        size.y = healthBarObject.sizeDelta.y * healthPercent;
        healthBarObjectFill.sizeDelta = size;
    }

    // Scoring Functions

    public void ChangeScore(int amount)
    {
        score += amount;

        if (amount != 0)
        {
            scoreAudio.volume = volume;
            scoreAudio.PlayOneShot(scoreAudioClip);
        }

        string scoreString = "Score: " + score;
        scoreObject.text = scoreString;
    }

    // Timer Functions

    private void UpdateTimerUI() // Keeping private because this is the only script that should really need it.
    {
        if (timerObject != null)
        {
            timerObject.text = "Round Time: " + ((int)timer);
        }
    }

    // Starts the timer
    public void StartTimer()
    {
        timerRunning = true;
        print("Starting Timer");
    }

    // Stops the timer
    public void StopTimer()
    {
        timerRunning = false;
        print("Stopping Timer");
    }

    // Resets the timer and turns it off
    public void ResetTimer()
    {
        timer = 0f;
        StopTimer();
        UpdateTimerUI();
    }

    // Resets the timer while keeping it going
    public void RestartTimer()
    {
        timer = 0f;
        StartTimer();
        UpdateTimerUI();
    }

    public void SetDifficulty(float amount)
    {
        difficulty = amount;
    }


}
