using UnityEngine;
using System.IO;

public class ManageScoreFile : MonoBehaviour
{
    public void WriteScoreFile(string playerName, int score)
    {
        string path = Application.dataPath + "/leaderboard.csv";

        if (!File.Exists(path))
        {
            File.WriteAllText(path, "name,score,time");
        }

        // CSV format: Name, Score, Time
        string content = $"\n{playerName},{score},{System.DateTime.Now}";

        File.AppendAllText(path, content);
    }
}