using UnityEngine;
using System.Collections;

public class BuildingDestruction : MonoBehaviour
{
    [Range(0f, 10f)] public int maxHealth = 3;
    private int currentHealth;
    [Range(0f, 25f)] public int scoreReward = 10; // Amount of score to give player when building is destroyed
    [Range(0f, 10f)] public int healthReward = 5; // Amount of HP to restore when a building is destroyed

    [Header("Animation Settings")]
    [Range(0.01f, 1f)] public float sinkDuration = 0.2f;

    public GameObject collapseSound;

    // Get building renderer to apply colors
    private Renderer buildingRenderer;
    private float initialHeight; // Store the initial height to calculate sink amount

    private Renderer[] childRenderers; // TODO: Make flashing affect all attached meshes

    private ParticleSystem damageParticles; // Particle system that spawns a bunch of gray particles at the building's base

    private MaterialPropertyBlock propBlock; // Use material property blocks to avoid z-fighting
    private static readonly int ColorID = Shader.PropertyToID("_BaseColor");

    private static GameObject smokePrefab;
    private static GameObject rubblePrefab;

    private Vector3 targetPosition;
    private Vector3 targetParticleLocalPos;
    private Coroutine sinkCoroutine;




    void Start()
    {
        currentHealth = maxHealth;

        // Load prefabs once and cache them
        if (smokePrefab == null) 
        {
            smokePrefab = Resources.Load<GameObject>("SmokeEffect");
        }
        
        if (rubblePrefab == null) 
        {
            rubblePrefab = Resources.Load<GameObject>("RubblePile");
        }

        // Grab the Renderer
        buildingRenderer = GetComponent<Renderer>();

        // Grab the particle system
        damageParticles = GetComponentInChildren<ParticleSystem>();

        // Save the starting color and height
        if (buildingRenderer != null)
        {
            propBlock = new MaterialPropertyBlock();
            initialHeight = buildingRenderer.bounds.size.y;
            targetPosition = transform.position;
            if (damageParticles != null) targetParticleLocalPos = damageParticles.transform.localPosition;
        }
    }

    public void TakeDamage()
    {
        currentHealth -= 1;

        if (buildingRenderer != null)
        {
            FlashColor();
        }

        if (damageParticles != null)
        {
            spawnBuildingParticles();
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
        // sink the building into the ground smoothly
        if (buildingRenderer != null && maxHealth > 0)
        {
            // Calculate distance to sink: Total Height divided by hits needed
            float sinkAmount = initialHeight / maxHealth;

            // Update targets
            targetPosition -= new Vector3(0, sinkAmount, 0);
            if (damageParticles != null)
            {
                targetParticleLocalPos += new Vector3(0, sinkAmount, 0);
            }

            // Start smooth animation
            if (sinkCoroutine != null) StopCoroutine(sinkCoroutine);
            sinkCoroutine = StartCoroutine(AnimateSink());
        }
    }

    IEnumerator AnimateSink()
    {
        Vector3 startPos = transform.position;
        Vector3 startParticleLocalPos = damageParticles != null ? damageParticles.transform.localPosition : Vector3.zero;
        float elapsed = 0;

        while (elapsed < sinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / sinkDuration);

            transform.position = Vector3.Lerp(startPos, targetPosition, t);
            if (damageParticles != null)
            {
                damageParticles.transform.localPosition = Vector3.Lerp(startParticleLocalPos, targetParticleLocalPos, t);
            }
            yield return null;
        }

        transform.position = targetPosition;
        if (damageParticles != null)
        {
            damageParticles.transform.localPosition = targetParticleLocalPos;
        }
    }

    void FlashColor()
    {
        buildingRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor(ColorID, Color.white);
        buildingRenderer.SetPropertyBlock(propBlock);

        Invoke(nameof(ResetColor), 0.1f);
    }

    void ResetColor()
    {
        buildingRenderer.SetPropertyBlock(null);
    }

    void spawnBuildingParticles()
    {
        if (damageParticles != null)
        {
            damageParticles.Stop(); // If the particles are already being emitted, stop them first

            damageParticles.Play(); // Emit particles
        }
        else
        {
            Debug.LogWarning("No particle system attached to object");
        }
    }


    void Collapse()
    {
        // play collapse sound
        if (collapseSound != null)
        {
            Instantiate(collapseSound);
        }

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

        Vector3 rubblePos = new Vector3(finalX_Z.x, 20.1f, finalX_Z.z);
        Vector3 smokePos = new Vector3(finalX_Z.x, 23.5f, finalX_Z.z);


        if (smokePrefab != null)
        {
            GameObject smoke = Instantiate(smokePrefab, smokePos, Quaternion.identity);
            Destroy(smoke, 10f); // free up RAM
        }

        if (rubblePrefab != null)
        {
            GameObject rubble = Instantiate(rubblePrefab, rubblePos, Quaternion.identity);
            Destroy(rubble, 45f); // free up RAM
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

        PlayerAttack playerAttack = FindFirstObjectByType<PlayerAttack>();
        if (playerAttack != null)
        {
            playerAttack.IncreaseUltimateScore(scoreReward);
        }

        Destroy(gameObject);
    }
}