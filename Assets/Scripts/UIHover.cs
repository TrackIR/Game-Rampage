using UnityEngine;
using UnityEngine.EventSystems;

public class UIHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Bounce Settings")]
    public float hoverHeight = 10f;   // How high it moves up
    public float moveSpeed = 10f;     // How fast it snaps up
    public bool continuousBob = true;
    public float bobSpeed = 5f;       // Speed of the floating
    public float bobAmount = 5f;      // Height of the floating

    private RectTransform rectTransform;
    private Vector3 originalPos;
    private bool isHovered = false;
    private float timeHovered = 0f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPos = rectTransform.anchoredPosition3D;
    }

    void Update()
    {
        Vector3 targetPos = originalPos;

        if (isHovered)
        {
            // Move up to the target height
            float yOffset = hoverHeight;

            // Add continuous bobbing if enabled
            if (continuousBob)
            {
                timeHovered += Time.deltaTime * bobSpeed;
                yOffset += Mathf.Sin(timeHovered) * bobAmount;
            }

            targetPos.y += yOffset;
        }
        else
        {
            // Reset bobbing time when not hovered so it starts fresh next time
            timeHovered = 0f;
        }

        // Smoothly move to the target position
        rectTransform.anchoredPosition3D = Vector3.Lerp(rectTransform.anchoredPosition3D, targetPos, Time.deltaTime * moveSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}