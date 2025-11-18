using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public SceneAsset gameScene;

    public void PlayGame()
    {
        string sceneName = gameScene.name;
        SceneManager.LoadScene(sceneName);
    }
}
