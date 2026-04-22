using UnityEngine;
using UnityEngine.InputSystem;

public class headTrigger : MonoBehaviour
{
    public GameSettings gameSettings;
    public GameObject player;
    public GameObject head;
    private PlayerInput input;
    private InputAction attackAction;
    private int animAttachHash;
    private Animator anim;
    public GameObject Canvas;
    private Canvas UImanager;

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
            UImanager = Canvas.GetComponent<Canvas>();
            UImanager.GetComponent<ManageUI>().SetTutorialText(attackAction.GetBindingDisplayString() + " to fix Robot");
             if (gameSettings.useTrackIR)
             {
                 input.TrackIR.Enable();
             }
             else
             {
                 input.KeyboardMouse.Enable();
             }
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
        UImanager.GetComponent<ManageUI>().SetTutorialText("Lean forward to move and find the rest of the robot located under the floating TrackIR logo.");
    }

    void OnAttack(InputAction.CallbackContext context)
    {

            head.SetActive(false);
            gameObject.SetActive(false);
            player.SetActive(true);
            CharacterController playerController = player.GetComponent<CharacterController>();
            playerController.enabled = false;
            anim = player.GetComponentInChildren<Animator>();
            if (anim != null)
            animAttachHash = Animator.StringToHash("Base Layer.HeadAttach");
            anim.SetTrigger("HeadAttach");
            
            GameManager.Instance.StartGamePhase();
    }
}
