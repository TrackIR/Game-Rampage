using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using NaturalPoint.TrackIR;
using Unity.VisualScripting;



[RequireComponent(typeof(CharacterController))]
public class movement : MonoBehaviour
{
    CharacterController controller;

    TrackIRComponent trackIR;


    public float gravity = -9.81f;
    public float speed = 10.0f;
    public float jumpPower = 2.0f;

    public GameObject TrackIRRoot;


    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool ShouldFaceMoveDirection = false;

    [SerializeField] private bool debugON = true;

    [SerializeField] private float headZThreshhold = 0.1f; // meters toward screen from neutral position to trigger forward movement
    [SerializeField] private float headXThreshold = 0.1f; // meters to the side from neutral position to trigger lateral movement
    [SerializeField] private float headRollThreshold = 23.0f; // degrees from neutral position to trigger roll movement
    [SerializeField] private float rollSpeed = 50.0f; // degrees per second when rolling
    [SerializeField] private bool invertRotation = false;

    Vector3 velocity;
    // store last computed head movement so other methods (OnGUI/UI) can read it
    private Vector3 headPos;

    private Quaternion headRot;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        trackIR = TrackIRRoot.GetComponent<TrackIRComponent>();
    }

    void zMove()
    {
        //when head moves forward past threshhold, move player forward
        if (Mathf.Abs(headPos.z) > headZThreshhold)
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
        // rotate player when head is rolled past threshold
        if (Mathf.Abs(headRot.z) > headRollThreshold)
        {
            // Determine rotation direction based on head roll and invert setting
            float rotDirection = (invertRotation ? -1f : 1f) * (headRot.z > 0 ? 1f : -1f);

            float rotAmount = rotDirection * rollSpeed * Time.deltaTime;

            transform.Rotate(0f, rotAmount, 0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        headPos = trackIR.LatestPosePosition;

        headRot = trackIR.LatestPoseOrientation;

        zMove();

        xMove();

        rotPlayer();

        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);

        // Keep player grounded
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }       
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        // Jumping: use jumpPower as desired jump height in meters
        if (jumpPressed && controller.isGrounded)
        {
            // v = sqrt(2 * g * h) where g is positive magnitude of gravity
            velocity.y = Mathf.Sqrt(jumpPower * -2f * gravity);
        }

        Debug.Log($"Head Z: {trackIR.LatestPosePosition.z:F3} m");
            
    }

    // Quick on-screen debug display (shows in Game view when Play is running)
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
