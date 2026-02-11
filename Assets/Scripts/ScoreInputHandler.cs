using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreInputHandler : MonoBehaviour
{
	[Header("UI References")]
	public TMP_InputField nameInputField;
	public Button submitButton;
	public TextMeshProUGUI buttonText; // reference for the button label

	private int finalScore;
	private bool hasSubmitted = false;

	// Called by PlayerHealth when the death menu opens
	public void Setup(int score)
	{
		finalScore = score;
		hasSubmitted = false;

		// Reset the UI state so it looks fresh every time player dies
		if (submitButton != null) submitButton.interactable = true;
		if (nameInputField != null)
		{
			nameInputField.text = "";
			nameInputField.interactable = true;
		}

		// Reset button text to "Submit"
		if (buttonText != null) buttonText.text = "Submit";
	}

	public void SubmitScore()
	{
		if (hasSubmitted) return;

		string playerName = nameInputField.text;

		if (string.IsNullOrEmpty(playerName))
		{
			playerName = "Unknown";
		}

		// Save to CSV
		ManageScoreFile writer = FindFirstObjectByType<ManageScoreFile>();
		if (writer != null)
		{
			writer.WriteScoreFile(playerName, finalScore);
		}

		hasSubmitted = true;

		// Disable input
		submitButton.interactable = false;
		nameInputField.interactable = false;

		if (buttonText != null)
		{
			buttonText.text = "Saved";
		}

		// Refresh leaderboard
		ReadLeaderboardFile reader = GetComponentInParent<ReadLeaderboardFile>();
		if (reader != null) reader.ReadFull();
	}
}