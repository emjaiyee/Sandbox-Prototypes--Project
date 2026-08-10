using UnityEngine;

public class InventoryItem
{
    public ItemData itemData;
    public Vector2Int originPosition;
    private bool isRotated;

    public RectTransform itemVisual;

    public InventoryItem(ItemData data)
    {
        itemData = data;
        isRotated = false;
    }

    public int GetWidth()
    {
        if (isRotated)
        {
            return itemData.gridHeight;
        }
        else
        {
            return itemData.gridWidth;
        }
    }

    public int GetHeight()
    {
        if (isRotated)
        {
            return itemData.gridWidth;
        }
        else
        {
            return itemData.gridHeight;
        }
    }

    public void Rotate()
    {
        isRotated = !isRotated;
    }

}
