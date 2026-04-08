using UnityEngine;
using TMPro;

public class KeyRemapper : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text buttonText;

    private bool isWaitingForInput = false;

    void Start()
    {
        // Display the currently saved key on the button when the menu opens
        string savedKey = PlayerPrefs.GetString("AttackKey", "Space");
        if (buttonText != null)
        {
            buttonText.text = savedKey;
        }
    }

    void Update()
    {
        // If the button was clicked, we wait for the next keyboard press
        if (isWaitingForInput && Input.anyKeyDown)
        {
            // Loop through all possible keys to see which one was just pressed
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    SaveNewKey(keyCode);
                    break;
                }
            }
        }
    }

    // You will link this function to the UI Button's "On Click()" event
    public void StartRemapping()
    {
        isWaitingForInput = true;
        if (buttonText != null)
        {
            buttonText.text = "Press A Key...";
        }
    }

    private void SaveNewKey(KeyCode newKey)
    {
        // Save the key as a string to PlayerPrefs
        PlayerPrefs.SetString("AttackKey", newKey.ToString());
        PlayerPrefs.Save(); // Force it to save immediately

        // Update the button text to show the new key
        if (buttonText != null)
        {
            buttonText.text = newKey.ToString();
        }

        isWaitingForInput = false; // Stop listening for inputs
    }
}