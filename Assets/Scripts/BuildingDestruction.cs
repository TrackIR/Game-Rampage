using UnityEngine;

public class BuildingDestruction : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public int scoreReward = 10;

    public int healthReward = 5; // Amount of HP to restore when a building is destroyed

    // We store the renderer and original color to handle flashing correctly
    private Renderer buildingRenderer;
    private Color originalColor;

    // Future Implementation: Add an array (public Mesh[] damageStages) here to swap meshes instead of scaling

    private Renderer[] childRenderers;

    private Color materialcolor;

    void Start()
    {
        materialcolor = GetComponent<Renderer>().material.color;
        currentHealth = maxHealth;

        // Grab the Renderer
        buildingRenderer = GetComponent<Renderer>();

        // Save the starting color
        if (buildingRenderer != null)
        {
            originalColor = buildingRenderer.material.color;
        }
    }

    public void TakeDamage()
    {
        currentHealth -= 1;

        if (buildingRenderer != null)
        {
            buildingRenderer.material.color = Color.red;
            Invoke("ResetColor", 0.1f);
        }

        //AudioManager.Instance.playAudio(AudioManager.Instance.buildingDestroy);
        

        // Update the look of the building
        UpdateDamageVisuals();

        if (currentHealth <= 0)
        {
            Collapse();
        }
    }

    void UpdateDamageVisuals()
    {
        // Get the height before shrinking
        float oldHeight = 0f;
        if (buildingRenderer != null)
        {
            oldHeight = buildingRenderer.bounds.size.y;
        }

        // Squash the building (Reduce Y scale by 20%)
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.8f, transform.localScale.z);

        // Lower the building so it stays on the ground
        if (buildingRenderer != null)
        {
            float newHeight = buildingRenderer.bounds.size.y;
            float heightLost = oldHeight - newHeight;

            // Move down by half the height lost to keep the base at the same level
            transform.position -= new Vector3(0, heightLost / 2f, 0);
        }

        // Future Implementation: GetComponent<MeshFilter>().mesh = damageStages[maxHealth - currentHealth - 1];
    }

    void ResetColor()
    {
        GetComponent<Renderer>().material.color = materialcolor;
    }


    void Collapse()
    {
        Vector3 finalX_Z = transform.position; // Default start

        // Is this building on a "BuildingTile"?
        if (transform.parent != null && transform.parent.CompareTag("BuildingTile"))
        {
            finalX_Z = transform.parent.position;
        }
        // Is it a standalone building?
        else if (buildingRenderer != null)
        {
            finalX_Z = buildingRenderer.bounds.center;
        }

        Vector3 rubblePos = new Vector3(finalX_Z.x, 1.26f, finalX_Z.z);
        Vector3 smokePos = new Vector3(finalX_Z.x, 1.5f, finalX_Z.z);

        GameObject smokePrefab = Resources.Load<GameObject>("SmokeEffect");
        GameObject rubblePrefab = Resources.Load<GameObject>("RubblePile");

        if (smokePrefab != null)
        {
            Instantiate(smokePrefab, smokePos, Quaternion.identity);
        }

        if (rubblePrefab != null)
        {
            Instantiate(rubblePrefab, rubblePos, Quaternion.identity);
        }

        // Score
        ManageUI uiManager = FindFirstObjectByType<ManageUI>();
        if (uiManager != null)
        {
            uiManager.ChangeScore(scoreReward);
        }

        // Heal
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        if (player != null)
        {
            player.Heal(healthReward);
        }

        Destroy(gameObject);
    }
}