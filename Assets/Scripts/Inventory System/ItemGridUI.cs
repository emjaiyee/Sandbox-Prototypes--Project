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

    private void Start()
    {
        float totalWidth = gridManager.gridWidth * cellSize;
        float totalHeight = gridManager.gridHeight * cellSize;

        gridRectTransform.sizeDelta = new UnityEngine.Vector2(totalWidth, totalHeight);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int gridPos = GetGridPosition(Input.mousePosition);
            InventoryItem newItem = new InventoryItem(testItemData);
            
            bool placed = gridManager.PlaceItem(newItem, gridPos.x, gridPos.y);

            if (placed)
            {
                CreateItemVisual(newItem);
            }
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

        float posX = gridRectTransform.rect.xMin + (item.originPosition.x * cellSize);
        float posY = gridRectTransform.rect.yMax - (item.originPosition.y * cellSize);
        rectTransform.anchoredPosition = new UnityEngine.Vector2(posX, posY);

        item.itemVisual = rectTransform;
    }
}
