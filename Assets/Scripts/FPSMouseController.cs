// First Person Mouse Controller script for Unity using the new Input System.
// Attach this script to the camera object in your scene. Make sure to set the playerBody reference to the player's body (usually the parent object of the camera) in the inspector.

using UnityEngine;
using UnityEngine.InputSystem;

public class FPSMouseController : MonoBehaviour
{
    [SerializeField] private InputAction mouseLookAction;
    [Range(0.1f, 9f)][SerializeField] private float sensitivity = 2f;
    [SerializeField] private float yRotationLimit = 88f;

    [SerializeField] private float smoothTime = 0.1f;

    // Reference to the player's body for horizontal rotation.
    [SerializeField] private Transform playerBody;

    Vector2 rotation = Vector2.zero;
    const string xAxis = "Mouse X";
    const string yAxis = "Mouse Y";


    void Start()
    {

        // Locks the cursor to the center of the screen and makes it invisible.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {

        Vector2 mouseInput = mouseLookAction.ReadValue<Vector2>();

        // Adjusts the rotation based on mouse input and sensitivity.
        rotation.x += mouseInput.x * sensitivity;
        rotation.y += mouseInput.y * sensitivity;

        // Clamps the vertical rotation to prevent flipping.
        rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);

        // Smoothly interpolates the rotation for a more fluid camera movement.
        // Applies the rotation to the camera and player body.
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(-rotation.y, 0f, 0f), smoothTime);
        playerBody.localRotation = Quaternion.Euler(0f, rotation.x, 0f);
    }

    void OnEnable()
    {
        mouseLookAction.Enable();
    }

    void OnDisable()
    {
        mouseLookAction.Disable();
    }
}
