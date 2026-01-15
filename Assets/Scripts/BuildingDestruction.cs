using UnityEngine;

public class BuildingDestruction : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public int scoreReward = 10; // How many points a building is worth

    private Color materialcolor;

    private Renderer[] childRenderers;

    void Start()
    {
        materialcolor = GetComponent<Renderer>().material.color;   
        currentHealth = maxHealth;

        childRenderers = GetComponentsInChildren<Renderer>(); // Get all children of object renderers (for red flash)
    }

    public void TakeDamage()
    {
        currentHealth -= 1;
        DamageVisual();


        if (currentHealth <= 0)
        {
            Collapse();
        }
    }

    private void DamageVisual()
    {
        foreach (Renderer r in childRenderers)
        {
            r.material.color = Color.red;
        }

        Invoke("ResetColor", 0.1f);
    }

    void ResetColor()
    {
        

        GetComponent<Renderer>().material.color = materialcolor;
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
