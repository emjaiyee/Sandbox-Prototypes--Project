using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [Header("Prompt")]
    [SerializeField] private string prompt = "Press E to pick up Item";

    [Header("Pickup Animation Settings")]
    [SerializeField] private float floatDuration = 0.25f;
    [SerializeField] private Vector3 heldPositionOffset = Vector3.zero;
    [SerializeField] private Vector3 heldRotationOffset = Vector3.zero;
    [SerializeField] private Vector3 targetScale = new Vector3(0.1f, 0.1f, 0.1f);

    private Collider objectCollider;
    private Rigidbody objectRigidbody;

    public string InteractionPrompt => prompt;

    private void Awake()
    {
        objectCollider = GetComponent<Collider>();
        objectRigidbody = GetComponent<Rigidbody>();
    }

    public void Interact(Transform handTransform)
    {
        // Disable collider and set kinematic to true.
        if (objectCollider != null) objectCollider.enabled = false;
        if (objectRigidbody != null) objectRigidbody.isKinematic = true;

        transform.SetParent(handTransform);

        StartCoroutine(MoveToHand());
    }


    private IEnumerator MoveToHand()
    {
        // Base position, rotation, and scale
        Vector3 startPosition = transform.localPosition;
        Quaternion startRotation = transform.localRotation;
        Vector3 startScale = transform.localScale;

        
        Quaternion targetRot = Quaternion.Euler(heldRotationOffset);
        float elapsedTime = 0f;

        // travel time to hand
        while (elapsedTime < floatDuration)
        {
            // run time
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / floatDuration;

            // Implement smooth transition of position, rotation, and scale
            transform.localPosition = Vector3.Lerp(startPosition, heldPositionOffset, t);
            transform.localRotation = Quaternion.Slerp(startRotation, targetRot, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            // Pause coroutine
            yield return null;
        }

        // New held position, rotation, scale
        transform.localPosition = heldPositionOffset;
        transform.localRotation = targetRot;
        transform.localScale = targetScale;
    }

    public void Drop(Vector3 dropPosition, Vector3 throwForce)
    {
        // Unparents from hand transform and drop position
        transform.SetParent(null);
        transform.position = dropPosition;

        // Enable Collider
        if (objectCollider != null) objectCollider.enabled = true;

        if (objectRigidbody != null)
        {
            // Remove kinematic
            objectRigidbody.isKinematic = false;

            // Throw math logic
            objectRigidbody.AddForce(throwForce, ForceMode.Impulse);
        }
    }
    
}