using UnityEngine;

public class BuildingDestruction : MonoBehaviour
{
    [Range(0f, 10f)] public int maxHealth = 3;
    private int currentHealth;
    [Range(0f, 25f)] public int scoreReward = 10; // Amount of score to give player when building is destroyed
    [Range(0f, 10f)] public int healthReward = 5; // Amount of HP to restore when a building is destroyed

    // Get building renderer to apply colors
    private Renderer buildingRenderer;
    private float initialHeight; // Store the initial height to calculate sink amount

    // Future implementation: Add an array (public Mesh[] damageStages) here to swap meshes instead of scaling

    private Renderer[] childRenderers; // TODO: Make flashing red affect all attached meshes, currently looks for one and makes it flash red.

    private MaterialPropertyBlock propBlock; // Use material property blocks to avoid z-fighting
    private static readonly int ColorID = Shader.PropertyToID("_BaseColor");




    void Start()
    {
        currentHealth = maxHealth;

        // Grab the Renderer
        buildingRenderer = GetComponent<Renderer>();

        // Save the starting color and height
        if (buildingRenderer != null)
        {
            propBlock = new MaterialPropertyBlock();
            initialHeight = buildingRenderer.bounds.size.y;

        }
    }

    public void TakeDamage()
    {
        currentHealth -= 1;

        if (buildingRenderer != null)
        {
            FlashRed();
        }



        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.playAudio(AudioManager.Instance.buildingDestroy);
        }

        // Update the look of the building
        UpdateDamageVisuals();

        if (currentHealth <= 0)
        {
            Collapse();
        }
    }

    void UpdateDamageVisuals()
    {
        // Instead of squashing, sink the building into the ground
        if (buildingRenderer != null && maxHealth > 0)
        {
            // Calculate distance to sink: Total Height divided by hits needed
            float sinkAmount = initialHeight / maxHealth;

            // Move the building down globally
            transform.position -= new Vector3(0, sinkAmount, 0);
        }
    }

    void FlashRed()
    {
        buildingRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor(ColorID, Color.red);
        buildingRenderer.SetPropertyBlock(propBlock);

        Invoke(nameof(ResetColor), 0.1f);
    }

    void ResetColor()
    {
        buildingRenderer.SetPropertyBlock(null);
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

        Vector3 rubblePos = new Vector3(finalX_Z.x, 20.2f, finalX_Z.z);
        Vector3 smokePos = new Vector3(finalX_Z.x, 21.5f, finalX_Z.z);

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