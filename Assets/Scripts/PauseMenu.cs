using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void PauseGame()
    {
        Debug.Log("Paused");
    }

    public void ResumeGame()
    {
        Debug.Log("Resume");
    }
}
