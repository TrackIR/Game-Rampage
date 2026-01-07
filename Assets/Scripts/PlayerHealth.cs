using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject Canvas;
    private Canvas UImanager;
    public bool isAlive = true;

    public string gameSceneName = "Main Menu";

    void Start()
    {
        // Set health at the start
        currentHealth = maxHealth;
        UImanager = Canvas.GetComponent<Canvas>();
    }

    // function that other scripts can call
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

    void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        isAlive = false;
        SceneManager.LoadScene(gameSceneName);
    }
}