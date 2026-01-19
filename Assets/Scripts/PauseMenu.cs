using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public SceneAsset gameScene;
    public SceneAsset mainMenuScene;

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

        string sceneName = gameScene.name;
        SceneManager.LoadScene(sceneName);
    }

    public void ExitToMenu()
    {
        Debug.Log("Exit to menu");

        Time.timeScale = 1f;
        string sceneName = mainMenuScene.name;
        SceneManager.LoadScene(sceneName);
    }
}
