using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using NaturalPoint.TrackIR;
using Unity.VisualScripting;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class movement : MonoBehaviour
{
    [Header("Game Settings")]
    public GameSettings gameSettings;

    [Header("Mouse Look (Absolute)")]
    public float yawRange = 180f;
    public float maxPitch = 120f;

    private float inputYaw;
    private float inputPitch;

    CharacterController controller;
    TrackIRComponent trackIR;
    public Camera trackIRCam;
    public Camera normal3rdCam;

    public float gravity = -9.81f;
    public float speed = 10.0f;
    public float jumpPower = 4.0f;

    public GameObject TrackIRRoot;
    public GameObject GroundSmashObject;
    private ParticleSystem GroundSmashEffect;
    public PlayerAudio playerAudio;

    // INPUT SYSTEM
    private PlayerInput input;
    private InputAction moveAction;
    private InputAction jumpAction;

    // TODO: first or third person toggle needs to be scene or just moved to be better

    private Transform cameraTransform;
    [SerializeField] private bool useTrackIR = true;
    [SerializeField] private bool debugON = true;
    [SerializeField] private float headZThreshold = 0.1f;
    [SerializeField] private float headXThreshold = 0.1f;
    [SerializeField] private float headRollThreshold = 23.0f;
    [SerializeField] private float headYawThreshold = 0.50f;
    [SerializeField] private float rollSpeed = 200.0f;
    [SerializeField] private bool invertRotation = false;
    [SerializeField] private bool rotateWithYaw;
    [SerializeField] private float jumpStartAngleThreshold = -5.0f; //must start below this angle to initiate jump
    [SerializeField] private float jumpEndAngleThreshold = -40.0f; //must exceed this angle to trigger jump
    [SerializeField] private float jumpYawThreshold = 10.0f; // degrees within neutral position for jump to be valid
    [SerializeField] private float groundPoundRadius = 5.0f; // radius of ground pound effect

    Vector3 velocity;
    private Vector3 headPos;
    private Quaternion headRot;
    private Queue<Quaternion> headRotQueue = new Queue<Quaternion>();

    private Animator anim;
    private int animJumpHash;
    private bool isAirborn = false;
    private Collider[] enemiesHit;

    // Custom remapped jump key preserved from HEAD
    private KeyCode jumpKey;

    void Awake()
    {
        input = new PlayerInput();

        if (gameSettings != null && gameSettings.useTrackIR)
        {
            moveAction = null; // TrackIR handles movement
            jumpAction = input.TrackIR.Jump;
        }
        else
        {
            moveAction = input.KeyboardMouse.Movement;
            jumpAction = input.KeyboardMouse.Jump;
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

        if (anim != null)
        {
            animJumpHash = Animator.StringToHash("Base Layer.Jump");
        }

        string savedJumpKey = PlayerPrefs.GetString("JumpKey", "Space");
        jumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), savedJumpKey);

        GroundSmashEffect = GroundSmashObject.GetComponent<ParticleSystem>();

        gameObject.SetActive(false);
    }

    void GroundPound()
    {
        Vector3 groundPoundPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        enemiesHit = Physics.OverlapSphere(groundPoundPosition, groundPoundRadius, LayerMask.GetMask("Enemy"));
        if (GroundSmashEffect != null)
        {
            GroundSmashEffect.gameObject.SetActive(true);
            GroundSmashEffect.Simulate(0f, true, true, true);
            GroundSmashEffect.Play(true);

            // play shockwave sfx
            playerAudio.PlayShockWave();
        }
        else
        {
            Debug.LogWarning("No ground smash particle system attached to object!");
        }
        foreach (var enemyCollider in enemiesHit)
        {
            EnemyHealth enemy = enemyCollider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(100);
                Debug.Log("Ground Pound hit enemy!");
            }
        }
    }

    void zMove()
    {
        //when head moves forward past threshhold, move player forward
        if (Mathf.Abs(headPos.z) > gameSettings.movementDeadzone/2)
        {
            Vector3 forward = transform.forward;
            forward.y = 0; // keep movement horizontal
            forward.Normalize();

            forward = headPos.z > 0 ? forward : -forward; // if head is forward, move forward else move backward

            Vector3 move = forward * speed * Time.deltaTime;
            controller.Move(move);

            // Calculate the percentage of movement (Less than 0.1 does idle, greater does walk)
            float speedPercent = move.magnitude;
            anim.SetFloat("Speed", speedPercent);
        }
    }

    void xMove()
    {
        //when head moves to the side past threshhold, move player sideways
        if (Mathf.Abs(headPos.x) > gameSettings.movementDeadzone/2)
        {
            Vector3 right = transform.right;
            right.y = 0; // keep movement horizontal
            right.Normalize();

            right = headPos.x < 0 ? -right : right; // if head is to left, move left else move right

            Vector3 move = right * speed * Time.deltaTime;
            controller.Move(move);

            // Calculate the percentage of movement (Less than 0.1 does idle, greater does walk)
            float speedPercent = move.magnitude;
            anim.SetFloat("Speed", speedPercent);
        }
    }

    void rotPlayer()
    {
        if (rotateWithYaw)
        {
            if (Mathf.Abs(headRot.y) > headYawThreshold)
            {
                float rotDirection = headRot.y > 0 ? 1f : -1f;
                float excessYaw = Mathf.Abs(headRot.y) - headYawThreshold;
                float rotAmount = rotDirection * rollSpeed * excessYaw * Time.deltaTime;
                transform.Rotate(0f, rotAmount, 0f);
            }
        }
        else
        {
            if (Mathf.Abs(headRot.z) > headRollThreshold)
            {
                float rotDirection = (invertRotation ? -1f : 1f) * (headRot.z > 0 ? 1f : -1f);
                float excessRoll = Mathf.Abs(headRot.z) - headRollThreshold;
                float rotAmount = rotDirection * rollSpeed * (excessRoll / 45f) * Time.deltaTime;
                transform.Rotate(0f, rotAmount, 0f);
            }
        }
    }

    void jump()
    {
        headRotQueue.Enqueue(headRot);
        if (headRotQueue.Count >= 30)
        {
            headRotQueue.Dequeue();
        }
        if (headRotQueue.Count > 20)
        {
            bool canJump = false;
            foreach (Quaternion rot in headRotQueue)
            {
                float pitch = rot.x * Mathf.Rad2Deg;
                float yaw = rot.y * Mathf.Rad2Deg;
                if (Mathf.Abs(yaw) > jumpYawThreshold) break;

                if (pitch > jumpStartAngleThreshold) canJump = true;

                if (canJump && pitch < jumpEndAngleThreshold)
                {
                    if (controller.isGrounded)
                    {
                        velocity.y = Mathf.Sqrt(jumpPower * -2f * gravity);
                        anim.SetTrigger("Jump");
                    }
                    headRotQueue.Clear();
                    break;
                }
            }
        }
    }

    void mouseRotatePlayer()
    {
        float nx = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;

        // treat like yaw input (same concept as headRot.y)
        float yawInput = nx;

        if (Mathf.Abs(yawInput) > headYawThreshold)
        {
            float rotDirection = yawInput > 0 ? 1f : -1f;

            float excess = Mathf.Abs(yawInput) - headYawThreshold;

            float rotAmount = rotDirection * rollSpeed * excess * Time.deltaTime;

            transform.Rotate(0f, rotAmount, 0f);
        }
    }

    void wasdMove()
    {
        float moveX = 0f;
        float moveZ = 0f;
        bool jumpPressed = false;

        // Try to read from the new input system if it's assigned
        if (moveAction != null)
        {
            Vector2 moveInput = moveAction.ReadValue<Vector2>();
            moveX = moveInput.x;
            moveZ = moveInput.y;
        }
        else
        {
            // Fallback to legacy input (Crucial if TrackIR fails mid-game and moveAction is null)
            moveX = Input.GetAxis("Horizontal");
            moveZ = Input.GetAxis("Vertical");
        }

        // Checking legacy remapped key first, falling back to new input system if needed
        if (Input.GetKeyDown(jumpKey))
        {
            jumpPressed = true;
        }
        else if (jumpAction != null && jumpAction.triggered)
        {
            jumpPressed = true;
        }

        Vector3 forward = transform.forward; // changed to player forward instead of camera for testing
        Vector3 right = transform.right; // changed to player right instead of camera right for testing //cameraTransform

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveZ + right * moveX;
        controller.Move(speed * Time.deltaTime * moveDirection);

        float speedPercent = moveDirection.magnitude;
        anim.SetFloat("Speed", speedPercent);

        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Fixed the bracket formatting conflict here
        if (jumpPressed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpPower * -2f * gravity);
        }
    }

    void Update()
    {

        // temp commented out these for testing
        //trackIRCam.enabled = useTrackIR;
        //normal3rdCam.enabled = !useTrackIR;
        //trackIRCam.GetComponent<AudioListener>().enabled = useTrackIR;
        //normal3rdCam.GetComponent<AudioListener>().enabled = !useTrackIR;
        //cameraTransform = Camera.main.transform;

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

            if (controller.isGrounded && velocity.y < 0f)
            {
                velocity.y = -2f;
            }
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            jump();
            if (jumpAction != null && jumpAction.triggered && controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpPower * -2f * gravity);
            }
        }
        else
        {
            mouseRotatePlayer();
            wasdMove();
        }

        if (!controller.isGrounded)
        {
            if (!isAirborn)
            {
                isAirborn = true;
            }
        }
        else
        {
            if (isAirborn)
            {
                GroundPound();
                isAirborn = false;
            }
        }
    }

    // on-screen debug display (shows in Game view when Play is running)
    void OnGUI()
    {
        if (debugON)
        {
            string s = string.Format("Head X: {0:F2}\nHead Y: {1:F2}\nHead Z: {2:F2}", headPos.x, headPos.y, headPos.z);
            GUI.Label(new Rect(10, 10, 220, 60), s);
        }
    }
}