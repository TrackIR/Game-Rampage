using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class ReadLeaderboardFile : MonoBehaviour
{
    public TextMeshProUGUI leaderboardTxt;
    public TextAsset leaderboardFile;

    public void ReadLatest()
    {
        var entries = ReadEntries();

        if (entries == null || entries.Count == 0)
        {
            leaderboardTxt.text = "No scores available";
            return;
        }

        // Latest = last valid entry in file order
        var latest = entries[entries.Count - 1];
        leaderboardTxt.text = $"{latest.score} at {latest.time}";
    }

    public void ReadFull()
    {
        var entries = ReadEntries();

        if (entries == null || entries.Count == 0)
        {
            leaderboardTxt.text = "No scores available";
            return;
        }

        // Sort by score descending
        entries.Sort((a, b) => b.score.CompareTo(a.score));

        StringBuilder output = new StringBuilder();
        foreach (var entry in entries)
        {
            output.AppendLine($"{entry.score} at {entry.time}");
        }

        leaderboardTxt.text = output.ToString();
    }

    // reads and parses the CSV
    private List<(int score, string time)> ReadEntries()
    {
        if (leaderboardFile == null)
            return null;

        var entries = new List<(int, string)>();
        string[] lines = leaderboardFile.text.Split('\n');

        // Skip header
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            string[] values = line.Split(',');
            if (values.Length != 2)
                continue;

            if (int.TryParse(values[1].Trim(), out int score))
            {
                string time = values[0].Trim();
                entries.Add((score, time));
            }
        }

        return entries;
    }
}
