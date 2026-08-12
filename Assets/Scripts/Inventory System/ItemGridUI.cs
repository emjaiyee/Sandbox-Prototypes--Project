using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public struct StoredItems
{
    public ItemData itemData;
    [Min(1)] public int quantity;
}

public class ItemGridUI : MonoBehaviour
{
    [SerializeField] private InventoryGrid gridManager;
    [SerializeField] private RectTransform gridRectTransform; //Anchor top-left
    [SerializeField] private float cellSize = 64f;

    [SerializeField] private RectTransform highlightRect;
    [SerializeField] private Image highlightImage;
    [SerializeField] private UnityEngine.Color validColor = new UnityEngine.Color(0f, 1f, 0f, 0.25f);
    [SerializeField] private UnityEngine.Color invalidColor = new UnityEngine.Color(0f, 1f, 0f, 0.25f);

    [SerializeField] private GameObject itemPrefab;

    [SerializeField] private List<StoredItems> storedItems = new List<StoredItems>();


    [SerializeField] private ItemData testItemData;

    // Tracks item state
    private InventoryItem heldItem;
    private Vector2Int heldItemOriginalPos;

    private void Awake()
    {
        InitializeHighlight();
    }

    private void Start()
    {
        float totalWidth = gridManager.gridWidth * cellSize;
        float totalHeight = gridManager.gridHeight * cellSize;

        gridRectTransform.sizeDelta = new UnityEngine.Vector2(totalWidth, totalHeight);

        InitializeInventoryItems();
    }

    private void Update()
    {

        if (heldItem != null)
        {
            UpdateHeldItemPosition();

            if (Input.GetMouseButtonDown(1))
            {
                RotateHeldItem();
            }
        }
        

        if (Input.GetMouseButtonDown(0))
        {
            if (heldItem == null)
            {
                Vector2Int clickedGridPos = GetGridPosition(Input.mousePosition);
                InventoryItem clickedItem = gridManager.GetItem(clickedGridPos.x, clickedGridPos.y);

                if (clickedItem != null)
                {
                    PickUpItem(clickedItem);
                }
            }
            else
            {
                Vector2Int targetPos = GetHeldItemGridPosition(heldItem);

                if (gridManager.PlaceItem(heldItem,targetPos.x, targetPos.y))
                {
                    SnapVisualToGrid(heldItem);
                    
                    if (heldItem.itemVisual.TryGetComponent<CanvasGroup>(out var canvasGroup))
                    {
                        canvasGroup.blocksRaycasts = true;
                    }

                    heldItem = null;
                }
            }
            
        }

        UpdateHighlight();
    }

    private void InitializeInventoryItems()
    {
        foreach (var item in storedItems)
        {
            if (item.itemData != null)
            {
                TryAddItem(item.itemData, item.quantity);
            }
        }
    }

    public bool TryAddItem(ItemData itemData, int quantity = 1)
    {
        InventoryItem newItem = new InventoryItem(itemData, quantity);

        if (gridManager.FindSpaceForItem(newItem, out Vector2Int position))
        {
            gridManager.PlaceItem(newItem, position.x, position.y);

            CreateItemVisual(newItem);
            SnapVisualToGrid(newItem);
            return true;
        }

        return false;
    }

    private void InitializeHighlight()
    {
        if (highlightRect == null)
        {
            GameObject hlObj = new GameObject("GridHighlight", typeof(RectTransform), typeof(Image));
            hlObj.transform.SetParent(gridRectTransform, false);

            highlightRect = hlObj.GetComponent<RectTransform>();
            highlightImage = hlObj.GetComponent<Image>();

            highlightRect.anchorMin = new UnityEngine.Vector2(0, 1);
            highlightRect.anchorMax = new UnityEngine.Vector2(0, 1);
            highlightRect.pivot = new UnityEngine.Vector2(0, 1);

            highlightImage.raycastTarget = false;
        }
        highlightRect.gameObject.SetActive(false);
    }

    private void UpdateHighlight()
    {
        if (heldItem == null)
        {
            highlightRect.gameObject.SetActive(false);
            return;
        }

        Vector2Int gridPos = GetHeldItemGridPosition(heldItem);
        bool isValid = gridManager.CanPlaceItem(heldItem, gridPos.x, gridPos.y);

        highlightRect.gameObject.SetActive(true);
        highlightRect.SetAsFirstSibling();

        highlightRect.sizeDelta = new UnityEngine.Vector2(
            heldItem.GetWidth() * cellSize,
            heldItem.GetHeight() * cellSize
        );

        float posX = gridRectTransform.rect.xMin + (gridPos.x * cellSize);
        float posY = gridRectTransform.rect.yMax - (gridPos.y * cellSize);
        highlightRect.anchoredPosition = new UnityEngine.Vector2(posX, posY);

        highlightImage.color = isValid ? validColor : invalidColor;
    }
    private void PickUpItem(InventoryItem item)
    {
        heldItem = item;
        heldItemOriginalPos = item.originPosition;

        gridManager.RemoveItem(item);

        item.itemVisual.SetAsLastSibling();

        if (item.itemVisual.TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void RotateHeldItem()
    {
        heldItem.Rotate();

        if (heldItem.itemVisual.TryGetComponent<ItemUIController>(out var controller))
        {
            controller.UpdateLayout(heldItem, cellSize);
        }

        UpdateHeldItemPosition();
    }
    
    private void UpdateHeldItemPosition()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gridRectTransform,
            Input.mousePosition,
            null,
            out UnityEngine.Vector2 localPoint
        );

        float width = heldItem.GetWidth() * cellSize;
        float height = heldItem.GetHeight() * cellSize;

        float posX = localPoint.x - (width / 2f);
        float posY = localPoint.y + (height / 2f);

        heldItem.itemVisual.anchoredPosition = new UnityEngine.Vector2(posX, posY);
        heldItem.itemVisual.SetAsLastSibling();
    }

    private void SnapVisualToGrid(InventoryItem item)
    {
        float posX = gridRectTransform.rect.xMin + (item.originPosition.x * cellSize);
        float posY = gridRectTransform.rect.yMax - (item.originPosition.y * cellSize);

        item.itemVisual.anchoredPosition = new UnityEngine.Vector2(posX, posY);
        item.itemVisual.localEulerAngles = UnityEngine.Vector3.zero;
        item.itemVisual.SetAsLastSibling();

        if (item.itemVisual.TryGetComponent<ItemUIController>(out var controller))
        {
            controller.UpdateLayout(item, cellSize);
        }
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

    public Vector2Int GetHeldItemGridPosition(InventoryItem item)
    {
        return GetGridPosition(Input.mousePosition);
    }

    private void CreateItemVisual(InventoryItem item)
    {
        GameObject obj = Instantiate(itemPrefab, gridRectTransform);

        obj.transform.localScale = UnityEngine.Vector3.one;
        UnityEngine.Vector3 localPos = obj.transform.localPosition;
        obj.transform.localPosition = new UnityEngine.Vector3(localPos.x, localPos.y, 0f);

        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        rectTransform.anchorMin = new UnityEngine.Vector2(0, 1);
        rectTransform.anchorMax = new UnityEngine.Vector2(0, 1);
        rectTransform.pivot = new UnityEngine.Vector2(0, 1);

        ItemUIController controller = obj.GetComponent<ItemUIController>();
        controller.Setup(item, cellSize);

        item.itemVisual = rectTransform;
    }

    private UnityEngine.Vector2 GetRotationOffset(InventoryItem item)
    {
        float origW = item.itemData.gridWidth * cellSize;
        float origH = item.itemData.gridHeight * cellSize;

        return item.RotationIndex switch
        {
            1 => new UnityEngine.Vector2(origH, 0),
            2 => new UnityEngine.Vector2(origW, -origH),
            3 => new UnityEngine.Vector2(0, -origW),
            _ => UnityEngine.Vector2.zero
        };
    }
}
