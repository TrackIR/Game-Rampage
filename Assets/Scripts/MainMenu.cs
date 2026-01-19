using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public string demoSceneName = "demo";

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(demoSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }
}
