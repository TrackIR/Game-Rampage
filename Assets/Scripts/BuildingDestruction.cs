using UnityEngine;

public class BuildingDestruction : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public int scoreReward = 10; // How many points a building is worth

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage()
    {
        currentHealth -= 1;

        // VISUAL: Flash red
        GetComponent<Renderer>().material.color = Color.red;
        Invoke("ResetColor", 0.1f);

        if (currentHealth <= 0)
        {
            Collapse();
        }
    }

    void ResetColor()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }

    void Collapse()
    {

        // Find the ManageUI script in the scene
        ManageUI uiManager = FindFirstObjectByType<ManageUI>();

        if (uiManager != null)
        {
            uiManager.ChangeScore(scoreReward);
        }
        else
        {
            Debug.LogWarning("ManageUI script not found in scene! Score not added.");
        }

        // 3. Destroy the building
        Destroy(gameObject);
    }
}
