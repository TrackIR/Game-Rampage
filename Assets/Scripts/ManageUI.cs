using UnityEngine;
using TMPro;

public class ManageUI : MonoBehaviour
{
    [Header("Game Settings")]
    public GameSettings gameSettings;

    // Health Variables
    [Header("Health Variables")]
    public TMP_Text healthObject;
    public RectTransform healthBarObject, healthBarObjectFill;
    public float maxHealth = 100f;

    [HideInInspector]
    public float currentHealth;

    // Timer Variables
    [Header("Timer Variables")]
    public TMP_Text timerObject;
    public float timeRemaining = 0;
    public bool timerIsRunning = false;

    // Score Variables
    [Header("Score Variables")]
    public TMP_Text scoreObject;
    public TMP_Text scoreOutline;

    public int score = 0;
    public float difficulty = 0f;

    // Trade Show Mode Flag
    private bool isTradeShow = false;
    private PlayerHealth playerHealth;

    void Start()
    {
        // Try to find the player to link to their health script
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        // DIFFICULTY & TIMER LOGIC
        if (gameSettings != null && gameSettings.difficulty == "Trade Show")
        {
            isTradeShow = true;
            timeRemaining = 120f; // 2 Minutes counting DOWN
        }
        else
        {
            isTradeShow = false;
            timeRemaining = 0f;   // Standard mode counts UP from 0
        }

        currentHealth = maxHealth;
        ChangeHealth(currentHealth);
        ChangeScore(0);
        timerIsRunning = true;
    }

    void Update()
    {
        // Timer Logic
        if (timerIsRunning)
        {
            if (isTradeShow)
            {
                // COUNT DOWN
                timeRemaining -= Time.deltaTime;

                if (timeRemaining <= 0)
                {
                    timeRemaining = 0;
                    timerIsRunning = false;

                    if (timerObject != null)
                    {
                        timerObject.text = "TIME'S UP!";
                        timerObject.color = Color.red;
                    }

                    // Instantly kill the player to trigger leaderboard
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(101);
                    }
                }
                else
                {
                    DisplayTime(timeRemaining);
                }
            }
            else
            {
                // COUNT UP
                timeRemaining += Time.deltaTime;
                DisplayTime(timeRemaining);
            }
        }
    }

    public void ChangeHealth(float health)
    {
        // Clamp the absolute health
        if (health < 0) health = 0;
        if (health > maxHealth) health = maxHealth;

        currentHealth = health;

        if (healthObject != null)
        {
            string healthString = Mathf.RoundToInt(currentHealth) + "/" + Mathf.RoundToInt(maxHealth);
            healthObject.text = healthString;
        }

        if (healthBarObjectFill != null && healthBarObject != null)
        {
            float healthPercent = currentHealth / maxHealth;

            Vector2 size = healthBarObjectFill.sizeDelta;
            size.y = healthBarObject.sizeDelta.y * healthPercent;
            healthBarObjectFill.sizeDelta = size;
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        if (timerObject != null)
        {
            if (isTradeShow)
            {
                timerObject.text = string.Format("Time Left: {0:0}:{1:00}", minutes, seconds);
            }
            else
            {
                timerObject.text = string.Format("Time: {0:0}:{1:00}", minutes, seconds);
            }
        }
    }

    public void ChangeScore(int scoreToAdd)
    {
        score += scoreToAdd;

        if (scoreObject != null) scoreObject.text = "Score: " + score;
        if (scoreOutline != null) scoreOutline.text = "Score: " + score;
    }
}