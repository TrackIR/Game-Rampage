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
        //leaderboardTxt.text = $"{latest.score} at {latest.time}";
        leaderboardTxt.text = $"Last Run: {latest.name} - {latest.score}";
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
            //output.AppendLine($"{entry.score} at {entry.time}");
            output.AppendLine($"{entry.name}: {entry.score} ({entry.time})");
        }

        leaderboardTxt.text = output.ToString();    
    }

    // reads and parses the CSV
    private List<(int score, string time, string name)> ReadEntries()
    {
        string path = Application.dataPath + "/leaderboard.csv";

        if (!File.Exists(path))
            return new List<(int, string, string)>();

        var entries = new List<(int, string, string)>();
        string[] lines = File.ReadAllLines(path);

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(',');

            // 3 columns: Time, Name, Score
            if (values.Length < 3) continue;

            // Score is now at index 2
            if (int.TryParse(values[2].Trim(), out int score))
            {
                string time = values[0].Trim();
                string name = values[1].Trim();
                entries.Add((score, time, name));
            }
        }

        return entries;
    }
}