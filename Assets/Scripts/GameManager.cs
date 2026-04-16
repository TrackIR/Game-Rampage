using UnityEngine;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[] gamePhaseObjects;
    public GameObject player;
    public GameObject indicatorObject;
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
    }

}
