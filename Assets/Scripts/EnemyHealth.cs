using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public AudioSource enemyDamageSound;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("Enemy hit for " + damage + " damage! Remaining Health: " + currentHealth);

        //Play Damage sound
        enemyDamageSound.Play(0);

        // flashes red
        GetComponent<Renderer>().material.color = Color.red;
        Invoke("ResetColor", 0.2f);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ResetColor()
    {
        GetComponent<Renderer>().material.color = Color.gray;
    }

    void Die()
    {
        Debug.Log("Enemy killed");
        Destroy(gameObject);
    }
}