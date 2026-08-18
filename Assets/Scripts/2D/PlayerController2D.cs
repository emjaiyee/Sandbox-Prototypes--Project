using UnityEngine;
using UnityEngine.InputSystem; // Required for the New Input System

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownNewInputController : MonoBehaviour
{
    [Header("Input Actions")]
    [Tooltip("Configure as Value -> Vector 2 in the Inspector")]
    [SerializeField] private InputAction moveAction;
    
    [Tooltip("Configure as Value -> Vector 2 (Pointer -> Position) in the Inspector")]
    [SerializeField] private InputAction mouseAction;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 mousePosition;
    private Camera mainCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        // Direct Inspector Actions must be manually enabled
        moveAction.Enable();
        mouseAction.Enable();
    }

    private void OnDisable()
    {
        // Best practice to clean up actions when disabled/destroyed
        moveAction.Disable();
        mouseAction.Disable();
    }

    private void Update()
    {
        // 1. Read Movement Vector (Normalized automatically depending on binding setup)
        moveInput = moveAction.ReadValue<Vector2>();

        // 2. Read Screen Space Mouse Position and translate to World Space
        if (mainCamera != null)
        {
            Vector2 screenMousePos = mouseAction.ReadValue<Vector2>();
            mousePosition = mainCamera.ScreenToWorldPoint(screenMousePos);
        }
    }

    private void FixedUpdate()
    {
        // 3. Move the character using physics
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);

        // 4. Rotate the character to look at the mouse
        Vector2 lookDir = mousePosition - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        
        rb.rotation = angle;
    }
}
