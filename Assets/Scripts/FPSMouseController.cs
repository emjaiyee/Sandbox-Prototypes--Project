using UnityEngine;
using UnityEngine.InputSystem;

public class FPSMouseController : MonoBehaviour
{
    [Range(0.1f, 9f)][SerializeField] private float sensitivity = 2f;
    [SerializeField] private float yRotationLimit = 88f;

    Vector2 rotation = Vector2.zero;
    const string xAxis = "Mouse X";
    const string yAxis = "Mouse Y";

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        rotation.x += Input.GetAxis(xAxis) * sensitivity;
        rotation.y += Input.GetAxis(yAxis) * sensitivity;

        rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);

        var xQuaternion = Quaternion.AngleAxis(rotation.x, Vector3.up);
        var yQuaternion = Quaternion.AngleAxis(rotation.y, Vector3.left);

        transform.localRotation = xQuaternion * yQuaternion;
    }
}
