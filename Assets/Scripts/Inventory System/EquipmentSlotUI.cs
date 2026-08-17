using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private EquipmentType slotType;
    [SerializeField] private RectTransform slotRectTransform;
    [SerializeField] private Image slotIconImage;

    public InventoryItem EquippedItem { get; private set; }
    public EquipmentType SlotType => slotType;

    private RectTransform equippedVisual;

    private void Awake()
    {
        if (slotRectTransform == null) slotRectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        DragDropManager dragManager = DragDropManager.Instance;

        // Try to equip held item
        if (dragManager.HeldItem != null)
        {
            if (CanEquip(dragManager.HeldItem))
            {
                EquipItem(dragManager.HeldItem);
            }
        }
        // Hand empty and slot occupied = Unequip
        else if (EquippedItem != null)
        {
            UnequipItem();
        }
    }

    public bool CanEquip(InventoryItem item)
    {
        if (item == null || item.Data == null) return false;
        return item.Data.EquipmentType == slotType;
    }

    public void EquipItem(InventoryItem newItem)
    {
        DragDropManager dragManager = DragDropManager.Instance;

        InventoryItem previousItem = EquippedItem;
        RectTransform previousVisual = equippedVisual;

        EquippedItem = newItem;
        equippedVisual = dragManager.heldItemVisual;

        if (equippedVisual != null)
        {
            equippedVisual.SetParent(slotRectTransform, false);
            equippedVisual.anchorMin = new Vector2(0.5f, 0.5f);
            equippedVisual.anchorMax = new Vector2(0.5f, 0.5f);
            equippedVisual.pivot = new Vector2(0.5f, 0.5f);
            equippedVisual.anchoredPosition = Vector2.zero;
            equippedVisual.localRotation = Quaternion.identity;
            equippedVisual.localRotation = Quaternion.identity;
            equippedVisual.localScale = Vector3.one;

            if (equippedVisual.TryGetComponent<ItemUIController>(out var controller))
            {
                controller.SetupForEquipment(newItem, 64f);
            }

            if (equippedVisual.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup.blocksRaycasts = true;
            }
        }

        dragManager.ClearHeldState();

        if (previousItem != null && previousVisual != null)
        {
            dragManager.PickUpItem(previousItem, null, previousVisual);
        }
    }

    public void UnequipItem()
    {
        if (EquippedItem == null) return;

        DragDropManager dragManager = DragDropManager.Instance;

        InventoryItem itemToPickup = EquippedItem;
        RectTransform visualToPickup = equippedVisual;

        EquippedItem = null;
        equippedVisual = null;

        dragManager.PickUpItem(itemToPickup, null, visualToPickup);
    }

    public void UpdateSlotVisual(InventoryItem item)
    {
        if (item != null && item.Data != null)
        {
            slotIconImage.sprite = item.Data.equipmentIcon != null
            ? item.Data.equipmentIcon
            :item.Data.inventoryIcon;

            slotIconImage.enabled = true;

            RectTransform rect = slotIconImage.rectTransform;
            rect.sizeDelta = new Vector2(64f, 64f);
            slotIconImage.preserveAspect = true;
        }
        else
        {
            slotIconImage.enabled = false;
        }
    }
}
