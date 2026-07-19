using UnityEngine;
using UnityEngine.InputSystem;

public class FPSMouseController : MonoBehaviour
{
    [SerializeField] InputAction lookAction;
    [SerializeField] float mouseSensitivity = 20f;
    [SerializeField] Transform playerBody;
    private float xRotation = 0f;

    private Rigidbody bodyRigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerBody != null)
        {
            bodyRigidbody = playerBody.GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseInput = lookAction.ReadValue<Vector2>();

        float mouseX = mouseInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (bodyRigidbody != null)
        {
            Quaternion deltaRotation = Quaternion.Euler(Vector3.up * mouseX);

            bodyRigidbody.MoveRotation(bodyRigidbody.rotation * deltaRotation);
        }

    }

    void OnEnable()
    {
        lookAction.Enable();
    }

    void OnDisable()
    {
        lookAction.Disable();
    }
}
