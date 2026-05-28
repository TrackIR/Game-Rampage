using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class ReadLeaderboardFile : MonoBehaviour
{
    public TextMeshProUGUI leaderboardTxt;

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
        leaderboardTxt.text = $"{latest.name}: {latest.score}";
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
            output.AppendLine($"{entry.name} - {entry.score}");
        }

        leaderboardTxt.text = output.ToString();
    }

    // reads and parses the CSV
    private List<(string name, int score, string time)> ReadEntries()
    {
        string path = Application.dataPath + "/leaderboard.csv";

        if (!File.Exists(path))
            return null;

        var entries = new List<(string, int, string)>();
        string[] lines = File.ReadAllLines(path);

        // Skip header
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            string[] values = line.Split(',');
            // expect Name, Score, Time
            if (values.Length < 2) continue;

            // Try parse score from column 1
            if (int.TryParse(values[1].Trim(), out int score))
            {
                string name = values[0].Trim();
                // If there is a 3rd column, use it for time, else use empty
                string time = (values.Length > 2) ? values[2].Trim() : "";
                entries.Add((name, score, time));
            }
        }

        return entries;
    }
}