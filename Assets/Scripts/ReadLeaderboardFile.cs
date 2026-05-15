using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class ReadLeaderboardFile : MonoBehaviour
{
    public TextMeshProUGUI leaderboardTxt;

    void Start()
    {
        SetupScrolling();
    }

    private void SetupScrolling()
    {
        if (leaderboardTxt == null) return;

        // 1. Ensure the text object can expand to fit all content
        ContentSizeFitter fitter = leaderboardTxt.gameObject.GetComponent<ContentSizeFitter>();
        if (fitter == null) fitter = leaderboardTxt.gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        // Set pivot to top so it expands downwards
        RectTransform textRect = leaderboardTxt.rectTransform;
        textRect.pivot = new Vector2(0.5f, 1f);
        textRect.anchorMin = new Vector2(0, 1);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.anchoredPosition = Vector2.zero;

        // 2. The parent needs to be the "Viewport" with a Mask and ScrollRect
        Transform viewport = leaderboardTxt.transform.parent;
        if (viewport != null)
        {
            // Add Mask to hide text that goes outside the background
            if (viewport.GetComponent<RectMask2D>() == null && viewport.GetComponent<Mask>() == null)
            {
                viewport.gameObject.AddComponent<RectMask2D>();
            }

            // Add ScrollRect to handle the actual scrolling
            ScrollRect scrollRect = viewport.GetComponent<ScrollRect>();
            if (scrollRect == null) scrollRect = viewport.gameObject.AddComponent<ScrollRect>();
            
            scrollRect.content = textRect;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.viewport = viewport as RectTransform;
        }
    }

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