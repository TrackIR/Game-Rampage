using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject Canvas;
    private Canvas UImanager;
    public bool isAlive = true;

    public SceneAsset gameScene;

    void Start()
    {
        // Set health at the start
        currentHealth = maxHealth;
        UImanager = Canvas.GetComponent<Canvas>();
    }

    // function that other scripts can call to Deal Damage
    public void TakeDamage(int damage)
    {
        // Reduce health
        currentHealth -= damage;
        UImanager.GetComponent<ManageUI>().ChangeHealth(-damage);
        Debug.Log(gameObject.name + " health: " + currentHealth);

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

        // Update UI (sending a positive number adds to the bar)
        UImanager.GetComponent<ManageUI>().ChangeHealth(amount);

        Debug.Log("Restored " + amount + " Health. Current: " + currentHealth);
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        isAlive = false;
        string sceneName = gameScene.name;
        SceneManager.LoadScene(sceneName);
    }
}