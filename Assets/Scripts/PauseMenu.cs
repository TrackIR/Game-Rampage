using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void PauseGame()
    {
        Debug.Log("Paused");
        Time.timeScale = 0f;
        Debug.Log(Time.timeScale);
    }

    public void ResumeGame()
    {
        Debug.Log("Resume");
        Time.timeScale = 1f;
        Debug.Log(Time.timeScale);
    }
}
