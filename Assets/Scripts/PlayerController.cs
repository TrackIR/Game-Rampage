using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    InputAction moveAction;
    InputAction jumpAction;

    private float verticalVelocity;

    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    private Rigidbody rb;
    private Vector2 moveInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        MovementControl();
    }

    void Move()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 moveVelocity = move * moveSpeed;
        Vector3 currentVelocity = rb.linearVelocity;

        // Maintain current Y velocity (for gravity/jumping)
        Vector3 velocity = new Vector3(moveVelocity.x, currentVelocity.y, moveVelocity.z);
        rb.linearVelocity = velocity;
    }

    void MovementControl()
    {
        Move();

        if (jumpAction.IsPressed())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpHeight, rb.linearVelocity.z); //Jetpack style action
            // Add a ground check for a regular jump
        }
    }
}
