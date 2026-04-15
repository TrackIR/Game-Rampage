using UnityEngine;
using UnityEngine.InputSystem;

public class headTrigger : MonoBehaviour
{
    public GameSettings gameSettings;
    public GameObject player;
    public GameObject head;
    private PlayerInput input;
    private InputAction attackAction;

    void Awake()
    {
        input = new PlayerInput();
        if (gameSettings.useTrackIR)
        {
            attackAction = input.TrackIR.Attack;
        }
        else
        {
            attackAction = input.KeyboardMouse.Attack;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHead"))
        {
            input.Enable();

            if (gameSettings.useTrackIR)
            {
                input.TrackIR.Enable();
            }
            else
            {
                input.KeyboardMouse.Enable();
            }
            attackAction.performed += OnAttack;
        }
    }
    void OnTriggerExit(Collider other)
    {
        input.Disable();
    }

    void OnAttack(InputAction.CallbackContext context)
    {
            head.SetActive(false);
            gameObject.SetActive(false);
            player.SetActive(true);
            GameManager.Instance.StartGamePhase();
    }
}
