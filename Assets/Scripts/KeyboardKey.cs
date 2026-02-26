using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KeyboardKey : MonoBehaviour
{
    public string character;
    private Button myButton;
    private TextMeshProUGUI myText;
    private ScoreInputHandler inputHandler;

    void Start()
    {
        myButton = GetComponent<Button>();
        inputHandler = FindFirstObjectByType<ScoreInputHandler>();

        // If character isn't set, grab it from the text label
        myText = GetComponentInChildren<TextMeshProUGUI>();
        if (myText != null && string.IsNullOrEmpty(character))
        {
            character = myText.text;
        }

        // Listen for click
        myButton.onClick.AddListener(HandleClick);
    }

    void HandleClick()
    {
        if (inputHandler != null)
        {
            inputHandler.AddLetter(character);
        }
    }
}