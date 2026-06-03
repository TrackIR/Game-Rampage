using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public EnemyAudio enemyAudio;
    public GameObject deathAudioPrefab;
    private int currentHealth;

    // store the renderers to handle the flashing correctly
    private Renderer[] enemyRenderers;
    private MaterialPropertyBlock propBlock;
    private static readonly int ColorID = Shader.PropertyToID("_BaseColor");

    private static GameObject deathVFX;
    private static GameObject corpsePrefab;

    void Start()
    {
        currentHealth = maxHealth;

        if (deathVFX == null) deathVFX = Resources.Load<GameObject>("EnemyDeathEffect");
        if (corpsePrefab == null) corpsePrefab = Resources.Load<GameObject>("EnemyCorpse");

        // Grab all Renderers in children
        enemyRenderers = GetComponentsInChildren<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    public void TakeDamage(int damage)
    {

        enemyAudio.PlayHurt();

        currentHealth -= damage;

        // Debug.Log("Enemy hit for " + damage + " damage! Remaining Health: " + currentHealth);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.playAudio(AudioManager.Instance.enemyHurt);
        }

        // Flash Red
        if (enemyRenderers != null && enemyRenderers.Length > 0)
        {
            propBlock.SetColor(ColorID, Color.red);
            foreach (Renderer r in enemyRenderers)
            {
                if (r != null) r.SetPropertyBlock(propBlock);
            }
            Invoke("ResetColor", 0.2f);
        }

        if (currentHealth <= 0)
        {
            Instantiate(deathAudioPrefab);
            Die();
        }
    }

    void ResetColor()
    {
        if (enemyRenderers != null)
        {
            foreach (Renderer r in enemyRenderers)
            {
                if (r != null) r.SetPropertyBlock(null);
            }
        }
    }

    void Die()
    {
        if (deathVFX != null)
        {
            // Spawn effect at chest height
            GameObject effect = Instantiate(deathVFX, transform.position + Vector3.up, Quaternion.identity);
            Destroy(effect, 2f);
        }

        if (corpsePrefab != null)
        {
            Vector3 bodyPos = new Vector3(transform.position.x, 1f, transform.position.z);

            // "90" on the X-axis tips it over. 
            // "transform.eulerAngles.y" keeps it facing the same direction it was looking.
            Quaternion flatRotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);

            // Capture the instantiated corpse in a variable
            GameObject corpse = Instantiate(corpsePrefab, bodyPos, flatRotation);

            // Destroy the corpse after 10 seconds to free up memory
            Destroy(corpse, 10f);
        }

        // Debug.Log("Enemy killed");
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        // No longer need to destroy material as we use PropertyBlocks
    }
}