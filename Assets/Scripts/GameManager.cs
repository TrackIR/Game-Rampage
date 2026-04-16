using UnityEngine;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[] gamePhaseObjects;
    public GameObject player;
    public GameObject indicatorObject;
    public GameObject InvisibleWalls;
    public GameObject Canvas;
    private Canvas UImanager;
    public GameObject TrackIRCam;
    public GameObject NormalCam;
    private cameraMovement3D camScript;
    private CinemachineCamera CinemachineCam;
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
        CinemachineCam = NormalCam.GetComponent<CinemachineCamera>();
        foreach (GameObject obj in gamePhaseObjects)
        {
            obj.SetActive(false);
        }
    }

    public void StartGamePhase()
    {
        if (tutorialCompleted) return;

        tutorialCompleted = true;

        camScript.playerObject = player;
        CinemachineCam.Follow = player.transform;

        foreach (GameObject obj in gamePhaseObjects)
        {
            obj.SetActive(true);
        }
        indicatorObject.SetActive(false);
        InvisibleWalls.SetActive(false);
        UImanager.GetComponent<ManageUI>().SetTutorialText("Now jump over the dump walls by quickly nodding up while moving forward.");
    }
}
