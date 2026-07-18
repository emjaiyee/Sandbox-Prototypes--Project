using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController3D : MonoBehaviour
{
    private Rigidbody rb;
    public InputAction moveAction;
    public InputAction jumpAction;
    public float moveSpeed = 7f;
    public float jumpForce = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() 
    {
        // Translates user inputs into digital value apparently.
        Vector2 inputVector = moveAction.ReadValue<Vector2>();

        // Line used to convert input into movement direction.
        // Also converts 2D inputs into 3D inputs.
        Vector3 moveDir = transform.right * inputVector.x + transform.forward * inputVector.y;

        // Velocity is deprecated. Use linearVelocity instead.
        // Line used to apply actual displacement.
        rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);
    }

    void Update()
    {
        // New input system, calls the specific input action. It's the new GetKeyDown.
        // It's also apparently frame-rate dependent. 
        if (jumpAction.WasPressedThisFrame())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // If not called then pressing the input actions will do absolutely nothing apparently.
    void OnEnable()
    {
        // Correct way of enabling components
        moveAction.Enable();
        jumpAction.Enable();
    }

     
    // If not called then it can still linger and possibly cause memory leak or something.
    void OnDisable()
    {
        // Correct way of enabling components
        moveAction.Disable();
        jumpAction.Disable();
    }


}