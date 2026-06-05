using UnityEngine;

public class SplashBounce : MonoBehaviour
{
    private Vector3 originalScale;
    public float bounceSpeed = 8f;
    public float bounceAmount = 0.1f;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float scaleModifier = Mathf.Sin(Time.time * bounceSpeed) * bounceAmount;
        transform.localScale = originalScale * (1f + scaleModifier);
    }
}
