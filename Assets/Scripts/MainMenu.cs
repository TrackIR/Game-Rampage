using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public SceneAsset gameScene;

    public void PlayGame()
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(current + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }
}
