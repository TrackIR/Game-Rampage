using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class ReadLeaderboardFile : MonoBehaviour
{
    public TextMeshProUGUI leaderboardTxt;
    public TextAsset leaderboardFile;

    public void UpdateLeaderboard()
    {
        if (leaderboardFile == null)
        {
            leaderboardTxt.text = "No leaderboard found";
            return;
        }

        string[] lines = leaderboardFile.text.Split('\n');

        List<(int score, string time)> entries = new List<(int, string)>();

        // skip header
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            string[] values = line.Split(',');
            if (values.Length != 2)
                continue;

            string time = values[0].Trim();

            if (int.TryParse(values[1].Trim(), out int score))
            {
                entries.Add((score, time));
            }
        }

        // sort by score descending
        entries.Sort((a, b) => b.score.CompareTo(a.score));

        StringBuilder output = new StringBuilder();
        foreach (var entry in entries)
        {
            output.AppendLine($"{entry.score} at {entry.time}");
        }

        leaderboardTxt.text = output.ToString();
    }
}
