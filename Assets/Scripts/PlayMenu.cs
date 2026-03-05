using TMPro;
using UnityEngine;

public class PlayMenu : MonoBehaviour
{
    public GameSettings settings;
    public string[] difficulty = { "Easy", "Normal", "Hard" };
    public int defaultDiff;

    public string[] map = { "Downtown" };
    public int defaultMap;

    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI mapText;

    void Start()
    {
        // Clamp to valid range and initialize text
        defaultDiff = Mathf.Clamp(defaultDiff, 0, difficulty.Length - 1);
        defaultMap = Mathf.Clamp(defaultMap, 0, map.Length - 1);

        difficultyText.text = difficulty[defaultDiff];
        mapText.text = map[defaultMap];

        settings.difficulty = difficulty[defaultDiff];
        settings.mapIndex = map[defaultMap];
    }

    public void IncreaseDifficulty()
    {
        defaultDiff = (defaultDiff + 1) % difficulty.Length;
        difficultyText.text = difficulty[defaultDiff];

        settings.difficulty = difficulty[defaultDiff];
    }

    public void DecreaseDifficulty()
    {
        defaultDiff--;
        if (defaultDiff < 0)
            defaultDiff = difficulty.Length - 1;

        difficultyText.text = difficulty[defaultDiff];
        
        settings.difficulty = difficulty[defaultDiff];
    }

    public void IncreaseMap()
    {
        defaultMap = (defaultMap + 1) % map.Length;
        mapText.text = map[defaultMap];

        settings.mapIndex = map[defaultMap];
    }

    public void DecreaseMap()
    {
        defaultMap--;
        if (defaultMap < 0)
            defaultMap = map.Length - 1;

        mapText.text = map[defaultMap];

        settings.mapIndex = map[defaultMap];
    }
}
