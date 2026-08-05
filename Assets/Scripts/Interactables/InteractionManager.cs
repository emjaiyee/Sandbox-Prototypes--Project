using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform; // Camera
    [SerializeField] private Transform handTransform; // Hand

    [Header("Raycast Settings")]
    [SerializeField] private float interactionDistance = 3f; // Interaction Range
    [SerializeField] private LayerMask interactableLayer; // Targets Interactable Layer Only

    [Header("Throw Settings")]
    [SerializeField] private float throwForce = 2f; // Strength of throw

    [Header("Input Actions")]
    [SerializeField] private InputAction interactAction; // Interaction Control
    [SerializeField] private InputAction dropAction; // Drop Control

    private IInteractable currentHoveredInteractable; // Interface
    private Item currentlyHeldItem;

    private void Update()
    {
        if (cameraTransform == null) return;

        DetectInteractable();
        HandleDrop();
    }

    public void DetectInteractable()
    {
        if (currentlyHeldItem != null)
        {
            currentHoveredInteractable = null;
            return;
        }

        //  Creates a Raycast to detect hit on item
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactionDistance, interactableLayer))
        {
            if (hitInfo.collider.TryGetComponent(out IInteractable interactable))
            {
                currentHoveredInteractable = interactable;

                if (interactAction.WasPressedThisFrame())
                {
                    currentHoveredInteractable.Interact(handTransform);

                    if (hitInfo.collider.TryGetComponent(out Item item))
                    {
                        currentlyHeldItem = item;
                    }

                    currentHoveredInteractable = null;
                }
                return;
            }
        }

        currentHoveredInteractable = null;
    }

    private void HandleDrop()
    {
        if (currentlyHeldItem == null) return;

        if (dropAction != null && dropAction.WasPressedThisFrame())
        {
            // Drop item in front of player.
            // Get drop position camera position + camera forward position.
            Vector3 dropPosition = cameraTransform.position + (cameraTransform.forward * 0.8f);

            // Get force camera forward position * throw force.
            Vector3 force = cameraTransform.forward * throwForce;

            // Drop held item logic
            currentlyHeldItem.Drop(dropPosition, force);

            // Allow currentlyHeldItem to be empty.
            currentlyHeldItem = null;
        }
    }

    // Just clears held item (null)
    public void ClearHeldItems()
    {
        currentlyHeldItem = null;
    }

    private void OnEnable()
    {
        interactAction.Enable();
        dropAction.Enable();
    }

    private void OnDisable()
    {
        interactAction.Disable();
        dropAction.Disable();
    }

    private void OnDrawGizmosSelected()
    {
        if (cameraTransform == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawRay(cameraTransform.position, cameraTransform.forward * interactionDistance);
    }
}