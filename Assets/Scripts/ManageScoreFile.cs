using UnityEngine;
using System.IO;

public class ManageScoreFile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
    //    WriteScoreFile(10000); // Temp value to test, use function in other scripts in practice
    //}

    public void WriteScoreFile(int score)
    {
        string path = Application.dataPath + "/leaderboard.csv";

        if (!File.Exists(path))
        {
            File.WriteAllText(path, $"time,score");
        }

        string content = $"\n{System.DateTime.Now},{score}";

        File.AppendAllText(path, content);
    }

}
