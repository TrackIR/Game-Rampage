using UnityEngine;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameSettings gameSettings;
    public GameObject[] gamePhaseObjects;
    public GameObject camTarget;
    public GameObject indicatorObject;
    public GameObject InvisibleWalls;
    public GameObject Canvas;
    private PlayerInput input;
    private InputAction jumpAction;
    private Canvas UImanager;
    public GameObject TrackIRCam;
    public GameObject NormalCam;
    private cameraMovement3D camScript;
    public bool tutorialCompleted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        UImanager = Canvas.GetComponent<Canvas>();
        camScript = TrackIRCam.GetComponent<cameraMovement3D>();
        foreach (GameObject obj in gamePhaseObjects)
        {
            obj.SetActive(false);
        }
        input = new PlayerInput();
        jumpAction = input.KeyboardMouse.Jump;
    }

    public void StartGamePhase()
    {
        if (tutorialCompleted) return;

        tutorialCompleted = true;

        foreach (GameObject obj in gamePhaseObjects)
        {
            obj.SetActive(true);
        }
        indicatorObject.SetActive(false);
        InvisibleWalls.SetActive(false);
        if (gameSettings.useTrackIR)
        {
            UImanager.GetComponent<ManageUI>().SetTutorialText("Nod up to jump over the wall");
        }
        else
        {
            UImanager.GetComponent<ManageUI>().SetTutorialText(jumpAction.GetBindingDisplayString() + " to jump over the wall");
        }
    }
}
