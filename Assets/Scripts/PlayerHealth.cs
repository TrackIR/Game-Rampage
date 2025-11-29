using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        // Set health at the start
        currentHealth = maxHealth;
    }

    // function that other scripts can call
    public void TakeDamage(int damage)
    {
        // Reduce health
        currentHealth -= damage;

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

        // For now, player object is destroyed (will add animation)
        Destroy(gameObject);
    }
}