using UnityEngine;
using System.Collections.Generic;

public class RobotOverheat : MonoBehaviour
{
    [Header("Overheat Settings")]
    public Color overheatColor = new Color(1.0f, 0.45f, 0.0f);
    public float maxEmissionIntensity = 5f;
    public float minEmissionIntensity = 0.1f;
    public float pulseSpeed = 3f; // Speed of the pulsation
    public float overheatThreshold = 0.9f; // Start pulsating at 90% charge

    private List<Renderer> robotRenderers = new List<Renderer>();
    private float currentPercent = 0f;
    private MaterialPropertyBlock propBlock;
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    // Base settings for calculating how much visuals should scale
    private float basePulseSpeed = 0.0f;
    private float baseMaxEmissionIntensity = 0.0f;
    private float baseMinEmissionIntensity = 0.0f;

    void Start()
    {
        propBlock = new MaterialPropertyBlock();

        // Get all renderers to apply the overheat effect
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            robotRenderers.Add(r);

            // In URP/Builds, we often need to ensure the keyword is active on the base materials
            // Doing it once here to help the renderer know it should look for emission
            foreach (Material m in r.sharedMaterials)
            {
                if (m != null) m.EnableKeyword("_EMISSION");
            }
        }

        // Store base values for scaling later
        basePulseSpeed = pulseSpeed;
        baseMaxEmissionIntensity = maxEmissionIntensity;
        baseMinEmissionIntensity = minEmissionIntensity;
    }

    void Update()
    {
        UpdateVisuals();
    }

    public void SetOverheatPercent(float percent)
    {
        currentPercent = percent;
    }

    private void UpdateVisuals()
    {
        if (robotRenderers.Count == 0) return;

        float intensity = 0f;

        // If above the threshold, start pulsating
        if (currentPercent >= overheatThreshold)
        {
            // Scale overheat visuals by current percent, building up to max
            // Normalizing percent from threshold..1.0 to 0..1.0 for better scaling
            float normalizedOverheat = (currentPercent - overheatThreshold) / (1f - overheatThreshold);

            float currentPulseSpeed = basePulseSpeed * (1f + normalizedOverheat);
            float currentMin = baseMinEmissionIntensity + (baseMaxEmissionIntensity * 0.2f * normalizedOverheat);
            float currentMax = baseMaxEmissionIntensity;

            // Use a sine wave to create a pulse effect
            float pulse = (Mathf.Sin(Time.time * currentPulseSpeed) + 1f) / 2f;

            // This lerps between min and max intensity
            intensity = Mathf.Lerp(currentMin, currentMax, pulse);
        }

        // Apply the overheat color with the calculated intensity
        // Using HDR color multiplication
        Color finalColor = overheatColor * intensity;

        // Update each renderer using the PropertyBlock
        // This is much more efficient and reliable in builds than modifying .material
        foreach (Renderer r in robotRenderers)
        {
            r.GetPropertyBlock(propBlock);
            propBlock.SetColor(EmissionColorId, finalColor);
            r.SetPropertyBlock(propBlock);
        }
    }
}
