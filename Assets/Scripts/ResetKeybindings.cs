using UnityEngine;

public class ResetKeybindings : MonoBehaviour
{
    public void ResetToDefaults()
    {
        // Delete the saved preferences to force them back to defaults
        PlayerPrefs.DeleteKey("AttackKey");
        PlayerPrefs.DeleteKey("JumpKey");
        PlayerPrefs.DeleteKey("UltimateKey");
        PlayerPrefs.Save();

        // Find all KeyRemapper scripts in the active menu and tell them to update their text
        KeyRemapper[] remappers = Object.FindObjectsByType<KeyRemapper>(FindObjectsSortMode.None);
        foreach (KeyRemapper remapper in remappers)
        {
            remapper.RefreshText();
        }
    }
}