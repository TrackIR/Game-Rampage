using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ManageUI : MonoBehaviour
{
    // Singleton so other scripts (like EnemyAI) can easily check the Threat Level
    public static ManageUI Instance;

    [Header("Game Settings")]
    public GameSettings gameSettings;

    [Header("UI Animation Settings")]
    public float uiAnimationSpeed = 1f; // Controls how fast the bars fill/drain
    private float currentHealthVisual;  // Tracks the smoothed health value
    private float currentUltVisual;     // Tracks the smoothed ult value
    private float targetUlt;            // Tracks the actual ult value
    private float maxUlt;               // Tracks the max ult value for percentage calculation

    [Header("Health Variables")]
    public TMP_Text healthObject;
    public RectTransform healthBarObject;
    public Image healthBarObjectFill;
    public float maxHealth = 100f;

    [HideInInspector]
    public float currentHealth;

    [Header("Ultimate Variables")]
    public TMP_Text ultObject;
    public RectTransform ultBarObject;
    public Image ultBarObjectFill;

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

        // Set initial visual values so they don't animate on scene load
        currentHealthVisual = maxHealth;
        currentUltVisual = 0f;
        targetUlt = 0f;
        maxUlt = 100f;

        // Timer starts at 0 and counts up for all modes now
        timeRemaining = 0f;
        currentHealth = maxHealth;
        ChangeHealth(currentHealth);

        // Initialize Ultimate bar to 0 at the start
        UpdateUlt(0, 100);

        // Force the UI to visually snap to default values immediately on start
        if (healthObject != null)
        {
            int startHealth = Mathf.RoundToInt(maxHealth);
            healthObject.text = startHealth.ToString();

            // Adjust Pos X based on if health is 100 (3 digits) or 0-99 (1-2 digits)
            Vector2 pos = healthObject.rectTransform.anchoredPosition;
            pos.x = (startHealth == 100) ? 140f : 174f;
            healthObject.rectTransform.anchoredPosition = pos;
        }
        if (healthBarObjectFill != null) healthBarObjectFill.fillAmount = 1f;

        if (ultObject != null)
        {
            ultObject.text = "0%";

            // Adjust Pos X for starting at 0%
            Vector2 ultPos = ultObject.rectTransform.anchoredPosition;
            ultPos.x = 175f;
            ultObject.rectTransform.anchoredPosition = ultPos;
        }
        if (ultBarObjectFill != null) ultBarObjectFill.fillAmount = 0f;

        ChangeScore(0);
        timerIsRunning = true;
    }

    void Update()
    {
        // Hide the timer/threat level text when the player dies
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
                // ESCALATION LOGIC: Threat Level increases every 20 seconds. Max is 5.
                wantedLevel = Mathf.Min(5, 1 + Mathf.FloorToInt(timeRemaining / 20f));
                DisplayWantedLevel();
            }
            else
            {
                DisplayTime(timeRemaining);
            }
        }

        // Animate Health
        if (currentHealthVisual != currentHealth)
        {
            currentHealthVisual = Mathf.MoveTowards(currentHealthVisual, currentHealth, Time.deltaTime * (uiAnimationSpeed * 20f));

            if (healthObject != null)
            {
                int displayHealth = Mathf.RoundToInt(currentHealthVisual);
                healthObject.text = displayHealth.ToString();

                // Adjust Pos X based on if health is 100
                Vector2 pos = healthObject.rectTransform.anchoredPosition;
                pos.x = (displayHealth == 100) ? 140 : 174f;
                healthObject.rectTransform.anchoredPosition = pos;
            }

            if (healthBarObjectFill != null)
            {
                healthBarObjectFill.fillAmount = currentHealthVisual / maxHealth;
            }
        }

        // Animate Ultimate
        if (currentUltVisual != targetUlt && maxUlt > 0)
        {
            currentUltVisual = Mathf.MoveTowards(currentUltVisual, targetUlt, Time.deltaTime * (uiAnimationSpeed * 20f));

            if (ultBarObjectFill != null)
            {
                ultBarObjectFill.fillAmount = Mathf.Clamp01(currentUltVisual / maxUlt);
            }

            if (ultObject != null)
            {
                float percent = (currentUltVisual / maxUlt) * 100f;
                int displayUlt = Mathf.RoundToInt(percent);
                ultObject.text = displayUlt.ToString() + "%";

                // Adjust Pos X for 100%, 10-99%, and 0-9%
                Vector2 ultPos = ultObject.rectTransform.anchoredPosition;
                if (displayUlt >= 100)
                {
                    ultPos.x = 140f;
                }
                else if (displayUlt >= 10)
                {
                    ultPos.x = 150f;
                }
                else
                {
                    ultPos.x = 175f;
                }
                ultObject.rectTransform.anchoredPosition = ultPos;
            }
        }
    }

    public void ChangeHealth(float health)
    {
        if (health < 0) health = 0;
        if (health > maxHealth) health = maxHealth;

        currentHealth = health;
    }

    public void UpdateUlt(float currentCharge, float maxCharge)
    {
        targetUlt = currentCharge;
        maxUlt = maxCharge;
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
                    timerObject.color = Color.red;
                    break;
                case 5:
                default:
                    // At max level
                    timerObject.text = "THREAT LEVEL: <color=red>MAXIMUM</color>";
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

    void OnDestroy()
    {
        // Remove the static reference so the Garbage Collector can sweep the old UI and Player
        if (Instance == this)
        {
            Instance = null;
        }
    }
}