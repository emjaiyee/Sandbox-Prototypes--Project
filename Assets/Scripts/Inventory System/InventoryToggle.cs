using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private GameObject inventoryPanel;

    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.I;
    [SerializeField] private bool startOpen = false;

    public bool IsOpen => inventoryPanel != null && inventoryPanel.activeSelf;

    private void Start()
    {
        SetInventoryState(startOpen);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (inventoryPanel == null) return;

        SetInventoryState(!inventoryPanel.activeSelf);
    }

    public void SetInventoryState(bool isOpen)
    {
        if (inventoryPanel == null) return;

        if (!isOpen && DragDropManager.Instance != null && DragDropManager.Instance.HeldItem != null)
        {
            DragDropManager.Instance.CancelDrag();
        }

        inventoryPanel.SetActive(isOpen);
    }
}