using UnityEngine;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameSettings gameSettings;
    public GameObject[] secondPhaseObjects;
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
        foreach (GameObject obj in secondPhaseObjects)
        {
            obj.SetActive(false);
        }
        input = new PlayerInput();
        jumpAction = input.KeyboardMouse.Jump;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void StartFirstGamePhase()
    {
        if (tutorialCompleted) return;

        tutorialCompleted = true;

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

    public void StartSecondGamePhase()
    {
        foreach (GameObject obj in secondPhaseObjects)
        {
            obj.SetActive(true);
        }
        UImanager.GetComponent<ManageUI>().timerIsRunning = true;
    }
}
