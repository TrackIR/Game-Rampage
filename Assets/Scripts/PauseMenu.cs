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

    public void ReloadGame()
    {
        Debug.Log("Restart game");
        Time.timeScale = 1f;

        // Force an immediate, blocking wipe and reload of the current active scene
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    public void ExitToMenu()
    {
        Debug.Log("Exit to menu");

        Time.timeScale = 1f;
        string sceneName = "Main Menu";
        SceneManager.LoadScene(sceneName);
    }
}
