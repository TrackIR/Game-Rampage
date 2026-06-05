using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MovementDeadzone : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameSettings gameSettings;
    [SerializeField] private TextMeshProUGUI valueText; // Drag your text component here

    void Start()
    {
        Slider movementDeadzone = GetComponent<Slider>();
        movementDeadzone.value = gameSettings.movementDeadzone;

        // Initialize the text display right away
        updateGameSettings(movementDeadzone.value);

        movementDeadzone.onValueChanged.AddListener(updateGameSettings);
    }

    public void updateGameSettings(float size)
    {
        gameSettings.movementDeadzone = size;
        Debug.Log($"Movement Deadzone updated to: {gameSettings.movementDeadzone}");

        // Short, single-line text update (formatted to 2 decimal places)
        if (valueText != null) valueText.text = $"{size:F2}";
    }
}
