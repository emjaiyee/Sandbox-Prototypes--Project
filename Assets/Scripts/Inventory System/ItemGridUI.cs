using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
    public struct InitialItemEntry
    {
        public ItemData itemData;
        public int quantity;
    }

public class ItemGridUI : MonoBehaviour
{
    [Header("Grid Configuration")]
    [SerializeField] private InventoryGrid gridManager;
    [SerializeField] private RectTransform gridRectTransform;
    [SerializeField] private float cellSize = 64f;

    [Header("Highlight Visuals")]
    [SerializeField] private RectTransform highlightRect;
    [SerializeField] private Image highlightImage;
    [SerializeField] private Color validColor = new Color(0f, 1f, 0f, 0.25f);
    [SerializeField] private Color invalidColor = new Color(1f, 0f, 0f, 0.25f);

    [Header("Item Prefab")]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private List<InitialItemEntry> storedItems = new List<InitialItemEntry>();

    // Mapping Model -> View
    private readonly Dictionary<InventoryItem, RectTransform> itemVisualMap = new Dictionary<InventoryItem, RectTransform>();

    private InventoryItem heldItem;

    private void OnEnable()
    {
        gridManager.OnItemPlaced += HandleItemPlaced;
        gridManager.OnItemRemoved += HandleItemRemoved;
        gridManager.OnItemRotated += HandleItemRotated;
    }

    private void OnDisable()
    {
        gridManager.OnItemPlaced -= HandleItemPlaced;
        gridManager.OnItemRemoved -= HandleItemRemoved;
        gridManager.OnItemRotated -= HandleItemRotated;
    }

    private void Start()
    {
        float totalWidth = gridManager.GridWidth * cellSize;
        float totalHeight = gridManager.GridHeight * cellSize;
        gridRectTransform.sizeDelta = new Vector2(totalWidth, totalHeight);

        InitializeHighlight();
        InitializeInventoryItems();
    }

    private void Update()
    {
        if (heldItem != null)
        {
            UpdateHeldItemPosition();

            if (Input.GetMouseButtonDown(1))
            {
                gridManager.RotateItem(heldItem);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }

        UpdateHighlight();
    }

    private void HandleMouseClick()
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
            Vector2Int targetPos = GetGridPosition(Input.mousePosition);

            if (gridManager.PlaceItem(heldItem, targetPos.x, targetPos.y))
            {
                if (itemVisualMap.TryGetValue(heldItem, out var visual))
                {
                    if (visual.TryGetComponent<CanvasGroup>(out var canvasGroup))
                    {
                        canvasGroup.blocksRaycasts = true;
                    }
                }
                heldItem = null;
            }
        }
    }

    private void PickUpItem(InventoryItem item)
    {
        heldItem = item;
        gridManager.RemoveItem(item);

        if (itemVisualMap.TryGetValue(item, out var visual))
        {
            visual.SetAsLastSibling();
            if (visual.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup.blocksRaycasts = false;
            }
        }
    }

    #region Model Event Callbacks

    private void HandleItemPlaced(InventoryItem item, Vector2Int position)
    {
        if (!itemVisualMap.TryGetValue(item, out var visual))
        {
            visual = CreateItemVisual(item);
        }

        SnapVisualToGrid(item, visual);
    }

    private void HandleItemRemoved(InventoryItem item, Vector2Int previousPosition)
    {
        if (heldItem != item && itemVisualMap.TryGetValue(item, out var visual))
        {
            Destroy(visual.gameObject);
            itemVisualMap.Remove(item);
        }
    }

    private void HandleItemRotated(InventoryItem item)
    {
        if (itemVisualMap.TryGetValue(item, out var visual))
        {
            if (visual.TryGetComponent<ItemUIController>(out var controller))
            {
                controller.UpdateLayout(item, cellSize);
            }
        }
    }

    #endregion

    #region Visual Mechanics & Positioning

    private RectTransform CreateItemVisual(InventoryItem item)
    {
        GameObject obj = Instantiate(itemPrefab, gridRectTransform);
        obj.transform.localScale = Vector3.one;

        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);

        if (obj.TryGetComponent<ItemUIController>(out var controller))
        {
            controller.Setup(item, cellSize);
        }

        itemVisualMap[item] = rectTransform;
        return rectTransform;
    }

    private void SnapVisualToGrid(InventoryItem item, RectTransform visual)
    {
        float posX = gridRectTransform.rect.xMin + (item.OriginPosition.x * cellSize);
        float posY = gridRectTransform.rect.yMax - (item.OriginPosition.y * cellSize);

        visual.anchoredPosition = new Vector2(posX, posY);
        visual.localEulerAngles = Vector3.zero;
        visual.SetAsLastSibling();

        if (visual.TryGetComponent<ItemUIController>(out var controller))
        {
            controller.UpdateLayout(item, cellSize);
        }
    }

    private void UpdateHeldItemPosition()
    {
        if (!itemVisualMap.TryGetValue(heldItem, out var visual)) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gridRectTransform,
            Input.mousePosition,
            null,
            out Vector2 localPoint
        );

        visual.anchoredPosition = localPoint;
        visual.SetAsLastSibling();
    }

    public Vector2Int GetGridPosition(Vector2 mousePosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gridRectTransform,
            mousePosition,
            null,
            out Vector2 localPoint
        );

        float relativeX = localPoint.x - gridRectTransform.rect.xMin;
        float relativeY = gridRectTransform.rect.yMax - localPoint.y;

        int x = Mathf.FloorToInt(relativeX / cellSize);
        int y = Mathf.FloorToInt(relativeY / cellSize);

        return new Vector2Int(x, y);
    }

    private void InitializeHighlight()
    {
        if (highlightRect == null)
        {
            GameObject hlObj = new GameObject("GridHighlight", typeof(RectTransform), typeof(Image));
            hlObj.transform.SetParent(gridRectTransform, false);

            highlightRect = hlObj.GetComponent<RectTransform>();
            highlightImage = hlObj.GetComponent<Image>();

            highlightRect.anchorMin = new Vector2(0, 1);
            highlightRect.anchorMax = new Vector2(0, 1);
            highlightRect.pivot = new Vector2(0, 1);

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

        Vector2Int gridPos = GetGridPosition(Input.mousePosition);
        bool isValid = gridManager.CanPlaceItem(heldItem, gridPos.x, gridPos.y);

        highlightRect.gameObject.SetActive(true);
        highlightRect.SetAsFirstSibling();

        highlightRect.sizeDelta = new Vector2(
            heldItem.GetWidth() * cellSize,
            heldItem.GetHeight() * cellSize
        );

        float posX = gridRectTransform.rect.xMin + (gridPos.x * cellSize);
        float posY = gridRectTransform.rect.yMax - (gridPos.y * cellSize);
        highlightRect.anchoredPosition = new Vector2(posX, posY);

        highlightImage.color = isValid ? validColor : invalidColor;
    }

    private void InitializeInventoryItems()
    {
        foreach (var item in storedItems)
        {
            if (item.itemData != null)
            {
                InventoryItem newItem = new InventoryItem(item.itemData, item.quantity);
                if (gridManager.FindSpaceForItem(newItem, out Vector2Int pos))
                {
                    gridManager.PlaceItem(newItem, pos.x, pos.y);
                }
            }
        }
    }

    #endregion
}