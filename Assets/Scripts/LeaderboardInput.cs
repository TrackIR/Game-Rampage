using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardInput : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Button submitButton;

    private bool hasSubmitted = false;

    public void SubmitScore()
    {
        if (hasSubmitted) return;

        // Get the Name
        string playerName = nameInput.text;
        if (string.IsNullOrEmpty(playerName)) playerName = "Anonymous";

        // Get the Score from ManageUI
        int finalScore = 0;
        ManageUI ui = FindFirstObjectByType<ManageUI>();
        if (ui != null)
        {
            finalScore = ui.score;
        }

        // Write to file
        ManageScoreFile writer = FindFirstObjectByType<ManageScoreFile>();
        if (writer != null)
        {
            writer.WriteScoreFile(playerName, finalScore);
            Debug.Log($"Score Submitted: {playerName} - {finalScore}");
        }
        else
        {
            Debug.LogError("ManageScoreFile not found! Make sure GameManager is in the scene.");
        }

        // Disable button
        hasSubmitted = true;
        submitButton.interactable = false;

        TextMeshProUGUI btnText = submitButton.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null) btnText.text = "Saved Score!";
    }
}