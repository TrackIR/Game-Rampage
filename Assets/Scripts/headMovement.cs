using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class headMovement : MonoBehaviour
{
    [Header("Game Settings")]
    public GameSettings gameSettings;
    CharacterController controller;
    TrackIRComponent trackIR;
    public Camera trackIRCam;
    public Camera normal3rdCam;
    public GameObject Canvas;
    private Canvas UImanager;
    public float gravity = -9.81f;
    public float speed = 10.0f;
    public GameObject TrackIRRoot;
    private PlayerInput input;
    private InputAction moveAction;
    private Transform cameraTransform;
    [SerializeField] private bool useTrackIR = true;
    [SerializeField] private bool debugON = true;
    [SerializeField] private float headZThreshold = 0.1f; // meters toward screen from neutral position to trigger forward movement
    [SerializeField] private float headXThreshold = 0.1f; // meters to the side from neutral position to trigger lateral movement
    [SerializeField] private float headRollThreshold = 23.0f; // degrees from neutral position to trigger roll movement
    [SerializeField] private float headYawThreshold = 0.50f; // radians from neutral position to trigger yaw movement
    [SerializeField] private float rollSpeed = 200.0f; // degrees per second when rolling
    [SerializeField] private bool invertRotation = false;
    [SerializeField] private bool rotateWithYaw;
    private Vector3 headPos;
    private Quaternion headRot;
    Vector3 velocity;
    private Animator anim;


    void Awake()
    {
        input = new PlayerInput();

        if (gameSettings != null && gameSettings.useTrackIR)
        {
            moveAction = null; // TrackIR handles movement
        }
        else
        {
            moveAction = input.KeyboardMouse.Movement;
        }
    }

    void OnEnable()
    {
        input.Enable();

        if (gameSettings != null && gameSettings.useTrackIR)
        {
            input.TrackIR.Enable();
        }
        else
        {
            input.KeyboardMouse.Enable();
        }
    }

    void OnDisable()
    {
        input.Disable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Read TrackIR setting from GameSettings if assigned
        if (gameSettings != null)
        {
            useTrackIR = gameSettings.useTrackIR;
        }

        controller = GetComponent<CharacterController>();

        if (TrackIRRoot != null)
        {
            trackIR = TrackIRRoot.GetComponent<TrackIRComponent>();
        }
        anim = gameObject.GetComponentInChildren<Animator>();

        UImanager = Canvas.GetComponent<Canvas>();

        ManageUI manageUI = UImanager.GetComponent<ManageUI>();

        manageUI.SetTutorialText("Lean forward to move and find the rest of the robot located under the floating TrackIR logo.");
    }
    void zMove()
    {
        //when head moves forward past threshhold, move player forward
        if (Mathf.Abs(headPos.z) > headZThreshold)
        {
            // Determine forward direction relative to player's orientation
            Vector3 forward = transform.forward;
            forward.y = 0; // keep movement horizontal
            forward.Normalize();

            forward = headPos.z > 0 ? forward : -forward; // if head is forward, move forward else move backward

            Vector3 move = forward * speed * Time.deltaTime;
            controller.Move(move);
        }
    }

    void xMove()
    {
        //when head moves to the side past threshhold, move player sideways
        if (Mathf.Abs(headPos.x) > headXThreshold)
        {
            // Determine right direction relative to player's orientation
            Vector3 right = transform.right;
            right.y = 0; // keep movement horizontal
            right.Normalize();

            right = headPos.x < 0 ? -right : right; // if head is to left, move left else move right

            Vector3 move = right * speed * Time.deltaTime;
            controller.Move(move);
        }
    }

    void rotPlayer()
    {
        if (rotateWithYaw)
        {
            // rotate player when head is yawed past threshold
            if (Mathf.Abs(headRot.y) > headYawThreshold)
            {
                // determine rotation direction
                float rotDirection = headRot.y > 0 ? 1f : -1f;

                // scale rotation speed by how far past threshold the head is
                float excessYaw = Mathf.Abs(headRot.y) - headYawThreshold;
                float rotAmount = rotDirection * rollSpeed * excessYaw * Time.deltaTime;

                transform.Rotate(0f, rotAmount, 0f);
            }
        }
        else
        {
            // rotate player when head is rolled past threshold
            if (Mathf.Abs(headRot.z) > headRollThreshold)
            {
                float rotDirection = (invertRotation ? -1f : 1f) * (headRot.z > 0 ? 1f : -1f);

                // scale rotation speed by how far past threshold the head roll is
                float excessRoll = Mathf.Abs(headRot.z) - headRollThreshold;

                float rotAmount = rotDirection * rollSpeed * (excessRoll / 45f) * Time.deltaTime; // dividing by 45 to normalize roll to a reasonable multiplier (adjust as needed)

                transform.Rotate(0f, rotAmount, 0f);
            }
        }
    }
    void wasdMove()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        float moveX = moveInput.x;
        float moveZ = moveInput.y;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveZ + right * moveX;
        controller.Move(speed * Time.deltaTime * moveDirection);

        float speedPercent = moveDirection.magnitude;

        if (speedPercent > 0)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }

        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }

    void Update()
    {
        trackIRCam.enabled = useTrackIR;
        normal3rdCam.enabled = !useTrackIR;
        trackIRCam.GetComponent<AudioListener>().enabled = useTrackIR;
        normal3rdCam.GetComponent<AudioListener>().enabled = !useTrackIR;
        cameraTransform = Camera.main.transform;

        if (useTrackIR && trackIR != null)
        {
            try
            {
                headPos = trackIR.LatestPosePosition;
                headRot = trackIR.LatestPoseOrientation;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"TrackIR read failed, falling back to WASD: {e.Message}");
                useTrackIR = false;
            }

            zMove();
            xMove();
            rotPlayer();

            // Keep player grounded
            if (controller.isGrounded && velocity.y < 0f)
            {
                velocity.y = -2f;
            }
            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
        else
        {
            wasdMove();
        }
    }

    void OnGUI()
    {
        if (debugON)
        {
            // small box in the top-left
            string s = string.Format("Head X: {0:F2}\nHead Y: {1:F2}\nHead Z: {2:F2}", headPos.x, headPos.y, headPos.z);
            GUI.Label(new Rect(10, 10, 220, 60), s);
        }
    }
}

