using UnityEngine;
using TMPro;

public class KeyRemapper : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text buttonText;

    [Header("Keybind Settings")]
    public string prefsKeyName = "AttackKey";
    public string defaultKey = "Q";      // The fallback key

    private bool isWaitingForInput = false;

    void Start()
    {
        RefreshText();
    }

    public void RefreshText()
    {
        // Display the currently saved key on the button
        string savedKey = PlayerPrefs.GetString(prefsKeyName, defaultKey);
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
        PlayerPrefs.SetString(prefsKeyName, newKey.ToString());
        PlayerPrefs.Save(); // Force it to save immediately

        // Update the button text to show the new key
        if (buttonText != null)
        {
            buttonText.text = newKey.ToString();
        }

        isWaitingForInput = false;
    }
}