using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public PlayerAudio playerAudio;
    public float maxHealth = 100f;
    public float startingHealth = 15f;
    private float currentHealth;
    public GameObject Canvas;
    public GameObject playerHealthCanvas;
    private Canvas UImanager;
    public bool isAlive = true;

    public GameObject deathMenu;
    public GameObject playMenu;

    public bool isInvincible = false;

    private Animator anim;
    private int animDamageHash;
    private int animDestroyHash;

    void Start()
    {
        // Set health at the start
        currentHealth = startingHealth;
        UImanager = Canvas.GetComponent<Canvas>();
        UImanager.GetComponent<ManageUI>().ChangeHealth(currentHealth);
        anim = gameObject.GetComponentInChildren<Animator>();

        if (anim != null)
        {
            animDamageHash = Animator.StringToHash("Base Layer.Damage");
            animDestroyHash = Animator.StringToHash("Base Layer.Destroy");
        }

        // Automatically find the player canvas
        if (playerHealthCanvas == null)
        {
            foreach (Canvas c in GetComponentsInChildren<Canvas>())
            {
                if (c.gameObject.name == "Canvas" && c.gameObject != Canvas)
                {
                    playerHealthCanvas = c.gameObject;
                    break;
                }
            }
        }
    }

    // function that other scripts can call to Deal Damage
    public void TakeDamage(float damage)
    {

        if (!isAlive || isInvincible) return;

        // ToDo: make player audio work!
        playerAudio.PlayHurt();

        // Reduce health
        currentHealth -= damage;

        if (UImanager != null)
        {
            UImanager.GetComponent<ManageUI>().ChangeHealth(currentHealth);
        }

        Debug.Log(gameObject.name + " health: " + currentHealth);

        // Play damage animation
        anim.SetTrigger("Damage");

        // Play damage sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.playAudio(AudioManager.Instance.playerHurt);
        }

        // Check if the player is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        // Increase health
        currentHealth += amount;

        // Cap health at maxHealth
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Player Heal Sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.playAudio(AudioManager.Instance.playerHeal);
        }

        // Send the new TOTAL health to the UI, not just the heal amount
        if (UImanager != null)
        {
            UImanager.GetComponent<ManageUI>().ChangeHealth(currentHealth);
        }

        Debug.Log("Restored " + amount + " Health. Current: " + currentHealth);
    }

    void Die()
    {
        if (!isAlive) return;
        isAlive = false;

        playerAudio.PlayDeath();
        anim.SetTrigger("Destroy");
        Debug.Log(gameObject.name + " has died!");

        if (playerHealthCanvas != null) playerHealthCanvas.SetActive(false);

        Invoke("ShowDeathMenu", 2.5f);
    }

    void ShowDeathMenu()
    {
        // Get the Score from ManageUI
        int finalScore = 0;
        if (UImanager != null)
        {
            finalScore = UImanager.GetComponent<ManageUI>().score;
        }

        if (playMenu) playMenu.SetActive(false);
        if (deathMenu)
        {
            deathMenu.SetActive(true);

            // Setup the input handler
            ScoreInputHandler inputHandler = deathMenu.GetComponent<ScoreInputHandler>();
            if (inputHandler != null)
            {
                inputHandler.Setup(finalScore);
            }

            // Update leaderboard visual
            ReadLeaderboardFile reader = deathMenu.GetComponentInChildren<ReadLeaderboardFile>();
            if (reader != null) reader.ReadFull();

            Time.timeScale = 0f;
        }
    }
}