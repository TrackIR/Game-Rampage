using UnityEngine;
using System.IO;

public class ManageScoreFile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WriteScoreFile(10000); // Temp value to test, use function in other scripts in practice
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void WriteScoreFile(int score)
    {
        string path = Application.dataPath + "/score.txt";

        if (!File.Exists(path))
        {
            File.WriteAllText(path, $"time, score");
        }

        string content = $"{System.DateTime.Now}, {score}";

        File.AppendAllText(path, content);
    }

}
