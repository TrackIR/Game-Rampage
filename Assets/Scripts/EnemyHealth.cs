using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    // store the renderer and original color to handle the flashing correctly
    private Renderer enemyRenderer;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;

        // Grab the Renderer
        enemyRenderer = GetComponent<Renderer>();

        // Save the starting color
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("Enemy hit for " + damage + " damage! Remaining Health: " + currentHealth);

        // Flash Red
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.red;
            Invoke("ResetColor", 0.2f);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ResetColor()
    {
        // 4. Revert to the exact color we saved in Start()
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = originalColor;
        }
    }

    void Die()
    {
        GameObject deathVFX = Resources.Load<GameObject>("EnemyDeathEffect");

        if (deathVFX != null)
        {
            // Spawn effect at chest height
            GameObject effect = Instantiate(deathVFX, transform.position + Vector3.up, Quaternion.identity);
            Destroy(effect, 2f);
        }

        GameObject corpsePrefab = Resources.Load<GameObject>("EnemyCorpse");
        if (corpsePrefab != null)
        {
            Vector3 bodyPos = new Vector3(transform.position.x, 1f, transform.position.z);

            // "90" on the X-axis tips it over. 
            // "transform.eulerAngles.y" keeps it facing the same direction it was looking.
            Quaternion flatRotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);

            Instantiate(corpsePrefab, bodyPos, flatRotation);
        }

        Debug.Log("Enemy killed");
        Destroy(gameObject);
    }
}