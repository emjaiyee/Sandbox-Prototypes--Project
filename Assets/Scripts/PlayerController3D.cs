using UnityEngine;
using UnityEngine.InputSystem; // New input system library

public class PlayerController3D : MonoBehaviour
{
    private Rigidbody rb;
    public InputAction moveAction;
    public InputAction jumpAction;
    public float moveSpeed = 7f;
    public float jumpForce = 5f;

    [Header ("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float sphereRadius = 0.3f;
    private bool isGrounded = true;

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
        // Don't forget do check ground status per frame.
        CheckGroundStatus();

        // New input system, calls the specific input action. It's the new GetKeyDown.
        // It's also apparently frame-rate dependent. 
        // And if isGrounded is also true.
        if (jumpAction.WasPressedThisFrame() && isGrounded )
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

    void CheckGroundStatus()
    {
        // Simple sphere offset.
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        // Sphere check if in contact with groundLayer.
        isGrounded = Physics.CheckSphere(spherePosition, sphereRadius, groundLayer);
    }

    void OnDrawGizmosSelected()
    {
        // Draws a sphere gizmo to visualize the ground check.
        Gizmos.color = Color.red;
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        Gizmos.DrawWireSphere(spherePosition, sphereRadius);
    }

}