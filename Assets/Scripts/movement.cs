using System;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class movement : MonoBehaviour
{
    CharacterController controller;
    public float gravity = -9.81f;
    public float speed = 10.0f;
    public float jumpPower = 2.0f;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool ShouldFaceMoveDirection = false;

    Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
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

        if(ShouldFaceMoveDirection && moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }


        // Move relative to the player's orientation
        //Vector3 move = transform.right * moveX + transform.forward * moveZ;
        //controller.Move(move * speed * Time.deltaTime);

        // Ensure a small downward velocity when grounded so CharacterController stays snapped to ground
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

}
