using UnityEngine;
using UnityEngine.UI;

public class CamSens : MonoBehaviour
{
    private Slider camSens;
    public GameSettings gameSettings;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        camSens = GetComponent<Slider>();
        camSens.value = gameSettings.camSens;
        camSens.onValueChanged.AddListener(updateGameSettings);
    }

    // Update is called once per frame
    public void updateGameSettings(float sensitivity)
    {
            gameSettings.camSens = sensitivity;
    }
}
