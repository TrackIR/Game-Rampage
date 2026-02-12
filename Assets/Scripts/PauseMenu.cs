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

        string sceneName = "SKO-demo";
        SceneManager.LoadSceneAsync(sceneName);
        Time.timeScale = 1f;
    }

    public void ExitToMenu()
    {
        Debug.Log("Exit to menu");

        Time.timeScale = 1f;
        string sceneName = "Main Menu";
        SceneManager.LoadScene(sceneName);
    }
}
