using System.Diagnostics;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class ItemGridUI : MonoBehaviour
{
    [SerializeField] private GridStateManager gridManager;
    [SerializeField] private RectTransform gridRectTransform; //Anchor top-left
    [SerializeField] private float cellSize = 64f;

    [SerializeField] private ItemData testItemData;

    // Tracks item state
    private InventoryItem heldItem;
    private Vector2Int heldItemOriginalPos;

    private void Start()
    {
        float totalWidth = gridManager.gridWidth * cellSize;
        float totalHeight = gridManager.gridHeight * cellSize;

        gridRectTransform.sizeDelta = new UnityEngine.Vector2(totalWidth, totalHeight);
    }

    private void Update()
    {
        if (heldItem != null)
        {
            UpdateHeldItemPosition();

            // Right click rotates hold item
            if (Input.GetMouseButtonDown(1))
            {
                RotateHeldItem();
            }
        }

        // Left click handling
        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int gridPos = GetGridPosition(Input.mousePosition);
            
            if (heldItem == null)
            {
                InventoryItem clickedItem = gridManager.GetItem(gridPos.x, gridPos.y);

                if (clickedItem != null)
                {
                    PickUpItem(clickedItem);
                }
                else if (testItemData != null)
                {
                    InventoryItem newItem = new InventoryItem(testItemData);
                    CreateItemVisual(newItem);
                    heldItem = newItem;
                    heldItemOriginalPos = gridPos;
                }
            }
            else
            {
                bool placed = gridManager.PlaceItem(heldItem, gridPos.x, gridPos.y);

                if (placed)
                {
                    SnapVisualToGrid(heldItem);
                    heldItem = null;
                }
            }
        }
    }

    private void PickUpItem(InventoryItem item)
    {
        heldItem = item;
        heldItemOriginalPos = item.originPosition;

        gridManager.RemoveItem(item);

        item.itemVisual.SetAsLastSibling();
    }

    private void RotateHeldItem()
    {
        heldItem.Rotate();

        float newWidth = heldItem.GetWidth() * cellSize;
        float newHeight = heldItem.GetHeight() * cellSize;
        heldItem.itemVisual.sizeDelta = new UnityEngine.Vector2(newWidth, newHeight);
    }
    
    private void UpdateHeldItemPosition()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gridRectTransform,
            Input.mousePosition,
            null,
            out UnityEngine.Vector2 localPoint
        );

        float widthOffset = (heldItem.GetWidth() * cellSize) / 2f;
        float heightOffset = (heldItem.GetHeight() * cellSize) / 2f;

        heldItem.itemVisual.anchoredPosition = new UnityEngine.Vector2(localPoint.x - widthOffset, localPoint.y + heightOffset);
    }

    private void SnapVisualToGrid(InventoryItem item)
    {
        float posX = gridRectTransform.rect.xMin + (item.originPosition.x * cellSize);
        float posY = gridRectTransform.rect.yMax - (item.originPosition.y * cellSize);
        item.itemVisual.anchoredPosition = new UnityEngine.Vector2(posX, posY);
    }

    // Relative to the local point of the panel pivot
    public Vector2Int GetGridPosition(UnityEngine.Vector2 mousePosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gridRectTransform,
            mousePosition,
            null,
            out UnityEngine.Vector2 localPoint
        );

        float relativeX = localPoint.x - gridRectTransform.rect.xMin;
        float relativeY = gridRectTransform.rect.yMax - localPoint.y;

        int x = Mathf.FloorToInt(relativeX / cellSize);
        int y = Mathf.FloorToInt(relativeY / cellSize);

        return new Vector2Int(x,y);
    }

    private void CreateItemVisual(InventoryItem item)
    {
        GameObject obj = new GameObject(item.itemData.itemName, typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(gridRectTransform, false);

        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        Image image = obj.GetComponent<Image>();

        image.sprite = item.itemData.icon;

        rectTransform.anchorMin = new UnityEngine.Vector2(0, 1);
        rectTransform.anchorMax = new UnityEngine.Vector2(0, 1);
        rectTransform.pivot = new UnityEngine.Vector2(0, 1);

        float width = item.GetWidth() * cellSize;
        float height = item.GetHeight() * cellSize;
        rectTransform.sizeDelta = new UnityEngine.Vector2(width, height);

        item.itemVisual = rectTransform;
    }
}
