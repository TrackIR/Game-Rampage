using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this for TextMeshPro

public class CamSens : MonoBehaviour
{
    public GameSettings gameSettings;
    [SerializeField] private TextMeshProUGUI valueText; // Drag your text component here

    void Start()
    {
        Slider camSens = GetComponent<Slider>();
        camSens.value = gameSettings.camSens;

        // Initialize the text display right away
        updateGameSettings(camSens.value);

        camSens.onValueChanged.AddListener(updateGameSettings);
    }

    public void updateGameSettings(float sensitivity)
    {
        gameSettings.camSens = sensitivity;
        Debug.Log($"Camera Sensitivity updated to: {gameSettings.camSens}");

        // Short, single-line text update (formatted to 2 decimal places)
        if (valueText != null) valueText.text = $"Sens: {sensitivity:F2}";
    }
}