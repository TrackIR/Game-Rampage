using UnityEngine;
using UnityEngine.UI;

public class TrackIRToggle : MonoBehaviour
{
    public GameSettings gameSettings;
    public Toggle trackIRToggle;

    void Start()
    {
        // When the menu loads, make sure the toggle visually matches the current setting
        if (gameSettings != null && trackIRToggle != null)
        {
            trackIRToggle.isOn = gameSettings.useTrackIR;

            // Add a listener so when the player clicks it, it updates the setting file
            trackIRToggle.onValueChanged.AddListener(UpdateTrackIRSetting);
        }
    }

    public void UpdateTrackIRSetting(bool isEnabled)
    {
        if (gameSettings != null)
        {
            gameSettings.useTrackIR = isEnabled;
            Debug.Log("TrackIR Enabled: " + gameSettings.useTrackIR);
        }
    }
}