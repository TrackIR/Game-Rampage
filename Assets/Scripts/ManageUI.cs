using UnityEngine;
using TMPro;

public class ManageUI : MonoBehaviour
{
    // Singleton so other scripts (like EnemyAI) can easily check the Threat Level
    public static ManageUI Instance;

    [Header("Game Settings")]
    public GameSettings gameSettings;

    [Header("Tutorial Variables")]
    public TMP_Text tutorialText;

    [Header("Health Variables")]
    public TMP_Text healthObject;
    public RectTransform healthBarObject, healthBarObjectFill;
    public float maxHealth = 100f;

    [HideInInspector]
    public float currentHealth;

    [Header("Timer & Escalation Variables")]
    public TMP_Text timerObject;
    public float timeRemaining = 0;
    public bool timerIsRunning = false;

    // The escalating threat level (1 to 5)
    public int wantedLevel = 1;

    [Header("Score Variables")]
    public TMP_Text scoreObject;
    public TMP_Text scoreOutline;

    public int score = 0;
    public float difficulty = 0f;

    public bool isTradeShow = false;
    private PlayerHealth playerHealth;
    private TutorialTextFade tutorialTextFade;

    void Awake()
    {
        // Set up the singleton instance
        Instance = this;
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        // Check if Trade Show mode is active
        if (gameSettings != null && gameSettings.difficulty == "Trade Show")
        {
            isTradeShow = true;
        }
        else
        {
            isTradeShow = false;
        }

        tutorialTextFade = tutorialText.GetComponent<TutorialTextFade>();

        // Timer starts at 0 and counts up for all modes
        timeRemaining = 0f;
        ChangeHealth(playerHealth.startingHealth);
        ChangeScore(0);
        timerIsRunning = true;
    }

    void Update()
    {
        if (currentHealth <= 0 && timerIsRunning)
        {
            timerIsRunning = false; // Stop all timer logic
            if (timerObject != null)
            {
                timerObject.text = ""; // Clears the text off the screen completely
            }
            return; // Exit the loop early
        }

        if (timerIsRunning)
        {
            timeRemaining += Time.deltaTime;

            if (isTradeShow)
            {
                // Threat Level increases every 20 seconds, max is 5
                wantedLevel = Mathf.Min(5, 1 + Mathf.FloorToInt(timeRemaining / 20f));
                DisplayWantedLevel();
            }
            else
            {
                DisplayTime(timeRemaining);
            }
        }
    }

    public void SetTutorialText(string text)
    {

        if (tutorialTextFade == null)
        {
            tutorialTextFade = tutorialText.GetComponent<TutorialTextFade>();
        }

        tutorialTextFade.FadeOut();
        tutorialTextFade.FadeIn(text);
    }

    public void ChangeHealth(float health)
    {
        if (health < 0) health = 0;
        if (health > maxHealth) health = maxHealth;

        currentHealth = health;

        if (healthObject != null)
        {
            healthObject.text = Mathf.RoundToInt(currentHealth) + "/" + Mathf.RoundToInt(maxHealth);
        }

        if (healthBarObjectFill != null && healthBarObject != null)
        {
            float healthPercent = currentHealth / maxHealth;
            Vector2 size = healthBarObjectFill.sizeDelta;
            size.y = healthBarObject.sizeDelta.y * healthPercent;
            healthBarObjectFill.sizeDelta = size;
        }
    }

    void DisplayWantedLevel()
    {
        if (timerObject != null)
        {
            // Dynamically change the text and color based on the current level
            switch (wantedLevel)
            {
                case 1:
                    timerObject.text = "THREAT LEVEL: 1";
                    timerObject.color = Color.white;
                    break;
                case 2:
                    timerObject.text = "THREAT LEVEL: 2";
                    timerObject.color = Color.yellow;
                    break;
                case 3:
                    timerObject.text = "THREAT LEVEL: 3";
                    timerObject.color = new Color(1f, 0.5f, 0f); // Orange
                    break;
                case 4:
                    timerObject.text = "THREAT LEVEL: 4";
                    timerObject.color = new Color(1.0f, 0.325f, 0.286f);
                    break;
                case 5:
                default:
                    // At max level, make it aggressive
                    timerObject.text = "THREAT LEVEL: <color=red>MAX</color>";
                    timerObject.color = Color.red;
                    break;
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        if (timerObject != null)
        {
            timerObject.text = string.Format("Time: {0:0}:{1:00}", minutes, seconds);
        }
    }

    public void ChangeScore(int scoreToAdd)
    {
        score += scoreToAdd;
        if (scoreObject != null) scoreObject.text = "Score: " + score;
        if (scoreOutline != null) scoreOutline.text = "Score: " + score;
    }
}