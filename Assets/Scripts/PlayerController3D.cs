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
        Vector2 inputVector = moveAction.ReadValue<Vector2>();

        Vector3 moveDir = transform.right * inputVector.x + transform.forward * inputVector.y;

        rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);
    }

    void Update()
    {
        if (jumpAction.WasPressedThisFrame())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }


}