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



    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool ShouldFaceMoveDirection = false;

    [SerializeField] private float headZThreshhold = 100.0f; // meters toward screen from neutral position to trigger forward movement

    Vector3 velocity;
    // store last computed head movement so other methods (OnGUI/UI) can read it
    Vector3 headPos;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        trackIR = GetComponent<TrackIRComponent>();
    }

    // Update is called once per frame
    void Update()
    {

        headPos = trackIR.transform.localPosition;

        
        //when head moves forward past threshhold, move player forward
        if (headPos.z > headZThreshhold)
        {
            // Determine forward direction relative to player's orientation
            Vector3 forward = transform.forward;
            forward.y = 0; // keep movement horizontal
            forward.Normalize();

            Vector3 move = forward * speed * Time.deltaTime;
            controller.Move(move);
            
        }
        
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        // Move relative to the player's orientation (not camera)
        //Vector3 move = transform.right * moveX + transform.forward * moveZ;
        //controller.Move(move * speed * Time.deltaTime);

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
            
    }

    // Quick on-screen debug display (shows in Game view when Play is running)
    // You can remove this or replace with a UI Text/TMP text field if you prefer.
    void OnGUI()
    {
        // small box in the top-left
        string s = string.Format("Head X: {0:F3}\nHead Y: {1:F3}\nHead Z: {2:F3}", headPos.x, headPos.y, headPos.z);
        GUI.Label(new Rect(10, 10, 220, 60), s);
    }

}
