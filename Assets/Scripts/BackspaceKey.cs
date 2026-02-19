using UnityEngine;
using UnityEngine.UI;

public class BackspaceKey : MonoBehaviour
{
    private Button myButton;
    private ScoreInputHandler handler;

    void Start()
    {
        myButton = GetComponent<Button>();
        handler = FindFirstObjectByType<ScoreInputHandler>();

        if (myButton != null)
        {
            myButton.onClick.AddListener(DoBackspace);
        }
    }

    void DoBackspace()
    {
        if (handler != null)
        {
            handler.Backspace();
        }
    }
}