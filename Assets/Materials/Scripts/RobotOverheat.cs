using UnityEngine;

public class RobotOverheat : MonoBehaviour
{
    [Header("Overheat Settings")]
    public Color overheatColor = new Color(1.0f, 0.45f, 0.0f);
    public float maxEmissionIntensity = 5f;
    public float minEmissionIntensity = 0.1f;
    public float pulseSpeed = 3f; // Speed of the pulsation
    public float overheatThreshold = 0.75f; // Start pulsating at 75% charge

    private Renderer[] robotRenderers;
    private float currentPercent = 0f;

    // Base settings for calculating how much visuals should scale
    // Visual effects will increase intensity as ultimate bar increases
    private float basePulseSpeed = 0.0f;
    private float baseMaxEmissionIntensity = 0.0f;
    private float baseMinEmissionIntensity = 0.0f;

    void Start()
    {
        // Get all renderers to apply the overheat effect
        robotRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in robotRenderers)
        {
            foreach (Material m in r.materials)
            {
                m.EnableKeyword("_EMISSION");
            }
        }

        basePulseSpeed = pulseSpeed;
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
        if (robotRenderers == null) return;

        float intensity = 0f;

        // If above the threshold, start pulsating
        
        if (currentPercent >= overheatThreshold)
        {

            // Scale overheat visuals by current percent, building up to max
            pulseSpeed = basePulseSpeed * currentPercent;
            minEmissionIntensity = baseMinEmissionIntensity * currentPercent;
            maxEmissionIntensity = baseMaxEmissionIntensity * currentPercent;

            // Use a sine wave to create a pulse effect
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            
            // This lerps between min and max intensity
            intensity = Mathf.Lerp(minEmissionIntensity, maxEmissionIntensity, pulse);
        }

        // Apply the overheat color with the calculated intensity
        Color finalColor = overheatColor * intensity;

        foreach (Renderer r in robotRenderers)
        {
            foreach (Material m in r.materials)
            {
                m.SetColor("_EmissionColor", finalColor);
            }
        }
    }
}
