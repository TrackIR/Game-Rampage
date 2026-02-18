using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public GameObject Canvas;
    private Canvas UImanager;
    public bool isAlive = true;

    public GameObject deathMenu;
    public GameObject playMenu;

    private Animator anim;
    private int animDamageHash;

    void Start()
    {
        // Set health at the start
        currentHealth = maxHealth;
        UImanager = Canvas.GetComponent<Canvas>();

        anim = gameObject.GetComponentInChildren<Animator>();

        if (anim != null)
        {
            animDamageHash = Animator.StringToHash("Base Layer.Damage");
        }
    }

    // function that other scripts can call to Deal Damage
    public void TakeDamage(float damage)
    {

        if (!isAlive) return;

        // Reduce health
        currentHealth -= damage;
        UImanager.GetComponent<ManageUI>().ChangeHealth(-damage);
        Debug.Log(gameObject.name + " health: " + currentHealth);

        // Play damage animation
        anim.SetTrigger("Damage");

        // Play damage sound
        //AudioManager.Instance.playAudio(AudioManager.Instance.playerHurt);

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
        //AudioManager.Instance.playAudio(AudioManager.Instance.playerHeal);

        // Update UI (sending a positive number adds to the bar)
        UImanager.GetComponent<ManageUI>().ChangeHealth(amount);

        Debug.Log("Restored " + amount + " Health. Current: " + currentHealth);
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        isAlive = false;

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