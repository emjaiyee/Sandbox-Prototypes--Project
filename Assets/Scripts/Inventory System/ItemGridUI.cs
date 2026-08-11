using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ItemGridUI : MonoBehaviour
{
    [SerializeField] private GridStateManager gridManager;
    [SerializeField] private RectTransform gridRectTransform; //Anchor top-left
    [SerializeField] private float cellSize = 64f;

    [SerializeField] private RectTransform highlightRect;
    [SerializeField] private Image highlightImage;
    [SerializeField] private UnityEngine.Color validColor = UnityEngine.Color.green;
    [SerializeField] private UnityEngine.Color invalidColor = UnityEngine.Color.red;


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
    }

    private void Update()
    {
        UpdateHighlight();

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
                heldItem.originPosition = gridPos;

                bool placed = gridManager.PlaceItem(heldItem, gridPos.x, gridPos.y);

                if (placed)
                {
                    SnapVisualToGrid(heldItem);
                    heldItem = null;
                }
            }
        }
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

        Vector2Int gridPos = GetGridPosition(Input.mousePosition);
        bool isValid = gridManager.PlaceItem(heldItem, gridPos.x, gridPos.y);

        highlightRect.gameObject.SetActive(true);
        highlightRect.SetAsLastSibling();

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
    }

    private void RotateHeldItem()
    {
        heldItem.Rotate();

        heldItem.itemVisual.localEulerAngles = new UnityEngine.Vector3(0, 0, heldItem.RotationAngle);

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

        float widthOffset = (heldItem.GetWidth() * cellSize) / 2f;
        float heightOffset = (heldItem.GetHeight() * cellSize) / 2f;

        UnityEngine.Vector2 rotOffset = GetRotationOffset(heldItem);

        float posX = localPoint.x - widthOffset + rotOffset.x;
        float posY = localPoint.y + heightOffset + rotOffset.y;

        heldItem.itemVisual.anchoredPosition = new UnityEngine.Vector2(posX, posY);
    }

    private void SnapVisualToGrid(InventoryItem item)
    {
        float posX = gridRectTransform.rect.xMin + (item.originPosition.x * cellSize);
        float posY = gridRectTransform.rect.yMax - (item.originPosition.y * cellSize);
        
        UnityEngine.Vector2 rotOffset = GetRotationOffset(item);

        item.itemVisual.anchoredPosition = new UnityEngine.Vector2(posX + rotOffset.x, posY + rotOffset.y);
        item.itemVisual.localEulerAngles = new UnityEngine.Vector3(0, 0, item.RotationAngle);
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

        float width = item.itemData.gridWidth * cellSize;
        float height = item.itemData.gridHeight * cellSize;
        rectTransform.sizeDelta = new UnityEngine.Vector2(width, height);

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
