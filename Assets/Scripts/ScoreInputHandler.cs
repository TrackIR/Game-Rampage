using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreInputHandler : MonoBehaviour
{
    [Header("UI References")]
	public TextMeshProUGUI nameDisplay;
	public Button submitButton;
	public TextMeshProUGUI submitButtonText;

	private int finalScore;
	private bool hasSubmitted = false;

	// current name being typed
	private string currentName = "";
	private const int MAX_CHARS = 3;

	// Run this on Start to ensure the placeholders appear immediately in the editor/game
	void Start()
	{
		UpdateDisplay();
	}

	public void Setup(int score)
	{
		finalScore = score;
		hasSubmitted = false;
		currentName = ""; // Reset to empty so placeholders appear
		UpdateDisplay();

		if (submitButton != null) submitButton.interactable = true;
		if (submitButtonText != null) submitButtonText.text = "SUBMIT";
	}

	// Called by KeyboardKey
	public void AddLetter(string letter)
	{
		if (hasSubmitted) return;
		if (currentName.Length >= MAX_CHARS) return;

		currentName += letter;
		UpdateDisplay();
	}

	public void Backspace()
	{
		if (hasSubmitted) return;
		if (currentName.Length > 0)
		{
			currentName = currentName.Substring(0, currentName.Length - 1);
			UpdateDisplay();
		}
	}

	void UpdateDisplay()
	{
		if (nameDisplay != null)
		{
			// Create an array of 3 characters
			char[] slots = new char[MAX_CHARS];

			for (int i = 0; i < MAX_CHARS; i++)
			{
				// If a letter typed for this slot, use it
				if (i < currentName.Length)
				{
					slots[i] = currentName[i];
				}
				// Otherwise, use underscore
				else
				{
					slots[i] = '_';
				}
			}

			// Join them with spaces "A _ _" or "_ _ _"
			nameDisplay.text = string.Join(" ", slots);
		}
	}

	public void SubmitScore()
	{
		Debug.Log("Submit Score Pressed");

		if (hasSubmitted) return;

		// If name is empty, default
		string finalName = string.IsNullOrEmpty(currentName) ? "UNK" : currentName;

		// Save to CSV
		ManageScoreFile writer = FindFirstObjectByType<ManageScoreFile>();
		if (writer != null)
		{
			writer.WriteScoreFile(finalName, finalScore);
		}

		hasSubmitted = true;

		if (submitButton != null) submitButton.interactable = false;
		if (submitButtonText != null) submitButtonText.text = "SAVED";

		// Refresh leaderboard
		ReadLeaderboardFile reader = GetComponentInParent<ReadLeaderboardFile>();
		if (reader != null) reader.ReadFull();
	}
}