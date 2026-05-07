using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class headTrigger : MonoBehaviour
{
    public GameSettings gameSettings;
    public GameObject player;
    public GameObject camTarget;
    public GameObject head;
    public cameraMovement3D cameraController;
    public Collider triggerCollider;
    private PlayerInput input;
    private InputAction attackAction;
    private int animAttachHash;
    private Animator anim;
    public GameObject Canvas;
    private Canvas UImanager;
    private movement playerMovement;
    private CharacterController playerController;
    private PlayerAttack playerAttack;
    private bool isAttaching;
    private Quaternion preAttachRotation;
    private bool preAttachRootMotion;

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

    void Start()
    {
        if (player != null)
        {
            playerMovement = player.GetComponent<movement>();
            playerController = player.GetComponent<CharacterController>();
            playerAttack = player.GetComponent<PlayerAttack>();
        }

        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<Collider>();
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
            attackAction.performed += OnAttack;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHead"))
        {
            attackAction.performed -= OnAttack;
            input.Disable();
            if (UImanager != null)
            {
                if (gameSettings.useTrackIR)
                {
                    UImanager.GetComponent<ManageUI>().SetTutorialText("Lean forward to move towards the floating TrackIR logo");
                }
                else
                {
                    UImanager.GetComponent<ManageUI>().SetTutorialText("Use WASD to move towards the floating TrackIR logo");
                }
            }
        }
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        if (isAttaching)
        {
            return;
        }

        isAttaching = true;

        head.SetActive(false);
        if (triggerCollider != null)
        {
            for (int i = 0; i < gameObject.GetComponentsInChildren<MeshRenderer>().Length; i++)
            {
                gameObject.GetComponentsInChildren<MeshRenderer>()[i].enabled = false;
            }
            triggerCollider.enabled = false;
        }
        player.SetActive(true);

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (playerAttack != null)
        {
            playerAttack.enabled = false;
        }

        anim = player.GetComponentInChildren<Animator>();
        if (anim != null)
        {
            animAttachHash = Animator.StringToHash("Base Layer.HeadAttach");
        }
        preAttachRotation = player.transform.rotation;
        if (anim != null)
        {
            preAttachRootMotion = anim.applyRootMotion;
            anim.applyRootMotion = false;
        }
        if (anim != null)
        {
            anim.SetTrigger("HeadAttach");
        }

        if (cameraController != null)
        {
            cameraController.playerObject = camTarget;
        }

        StartCoroutine(FinishAttach());
        GameManager.Instance.StartGamePhase();
    }

    private IEnumerator FinishAttach()
    {
        if (anim != null)
        {
            while (!anim.GetCurrentAnimatorStateInfo(0).IsName("HeadAttach"))
            {
                yield return null;
            }

            while (anim.GetCurrentAnimatorStateInfo(0).IsName("HeadAttach") &&
                   anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }
        }

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        if (cameraController != null)
        {
            cameraController.playerObject = player;
            cameraController.centerOffset = new Vector3(0f, 24f, 0f);
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (playerAttack != null)
        {
            playerAttack.enabled = true;
        }

        player.transform.rotation = preAttachRotation;
        if (anim != null)
        {
            anim.applyRootMotion = preAttachRootMotion;
        }


        isAttaching = false;
    }
}
