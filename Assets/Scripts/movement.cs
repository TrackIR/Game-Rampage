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
    CharacterController controller;

    TrackIRComponent trackIR;

    public Camera trackIRCam;

    public Camera normal3rdCam;

    public float gravity = -9.81f;
    public float speed = 10.0f;
    public float jumpPower = 2.0f;

    public GameObject TrackIRRoot;

    // TODO: first or third person toggle needs to be scene or just moved to be better

    private Transform cameraTransform;
    [SerializeField] private bool useTrackIR = true;
    [SerializeField] private bool debugON = true;
    [SerializeField] private float headZThreshold = 0.1f; // meters toward screen from neutral position to trigger forward movement
    [SerializeField] private float headXThreshold = 0.1f; // meters to the side from neutral position to trigger lateral movement
    [SerializeField] private float headRollThreshold = 23.0f; // degrees from neutral position to trigger roll movement
    [SerializeField] private float headYawThreshold = 0.50f; // radians from neutral position to trigger yaw movement
    [SerializeField] private float rollSpeed = 50.0f; // degrees per second when rolling
    [SerializeField] private bool invertRotation = false;
    [SerializeField] private bool rotateWithYaw;
    [SerializeField] private float jumpStartAngleThreshold = -5.0f; //must start below this angle to initiate jump
    [SerializeField] private float jumpEndAngleThreshold = -25.0f; //must exceed this angle to trigger jump
    [SerializeField] private float jumpYawThreshold = 10.0f; // degrees within neutral position for jump to be valid

    Vector3 velocity;
    private Vector3 headPos;
    private Quaternion headRot;
    private Queue<Quaternion> headRotQueue = new Queue<Quaternion>();


    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (TrackIRRoot != null)
        {
            trackIR = TrackIRRoot.GetComponent<TrackIRComponent>();
        }
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
                // Determine rotation direction based on head yaw and invert setting
                float rotDirection = headRot.y > 0 ? 1f : -1f;

                float rotAmount = rotDirection * rollSpeed * Time.deltaTime;

                transform.Rotate(0f, rotAmount, 0f);
            }
        }
        else
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
    }

    void jump()
    {
        // save head rotation data from last 60 frames to determine jump gesture
        headRotQueue.Enqueue(headRot);
        if (headRotQueue.Count >= 30)
        {
            headRotQueue.Dequeue();
        }
        if (headRotQueue.Count > 20)
        {
            // check if jump gesture is made
            bool canJump = false;
            foreach (Quaternion rot in headRotQueue)
            {
                float pitch = rot.x * Mathf.Rad2Deg;
                float yaw = rot.y * Mathf.Rad2Deg;
                if (Mathf.Abs(yaw) > jumpYawThreshold)
                { //can't jump if looking to side too much
                    break;
                }
                if (pitch > jumpStartAngleThreshold)
                {
                    canJump = true; //head was down enough to start jump
                }
                if (canJump && pitch < jumpEndAngleThreshold)
                {
                    // trigger jump
                    if (controller.isGrounded)
                    {
                        velocity.y = Mathf.Sqrt(jumpPower * -2f * gravity);
                    }
                    headRotQueue.Clear(); //reset queue after jump
                    break; //exit loop after jump because we don't need to keep checking
                }
            }
        }
    }

    void wasdMove()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveZ + right * moveX;
        controller.Move(moveDirection * speed * Time.deltaTime);

        Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);

        // Keep player grounded
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (jumpPressed && controller.isGrounded)
        {
            // v = sqrt(2 * g * h) where g is positive magnitude of gravity
            velocity.y = Mathf.Sqrt(jumpPower * -2f * gravity);
        }
    }

    // Update is called once per frame
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
            jump();
        }
        else
        {
            wasdMove();
        }
    }

    // on-screen debug display (shows in Game view when Play is running)
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
