using UnityEngine;

public class InventoryItem
{
    public ItemData itemData;
    public Vector2Int originPosition;

    private bool isRotated = false;
    public bool IsRotated => isRotated;
    private int rotationIndex;

    public RectTransform itemVisual;

    public int quantity = 1;

    public InventoryItem(ItemData data, int initialQuantity = 1)
    {
        itemData = data;
        quantity = initialQuantity;
        isRotated = false;
    }

    public int GetWidth()
    {
        return (rotationIndex % 2 == 0) ? itemData.gridWidth : itemData.gridHeight;
    }

    public int GetHeight()
    {
        return (rotationIndex % 2 == 0) ? itemData.gridHeight : itemData.gridWidth;
    }

    public void Rotate()
    {
        rotationIndex = (rotationIndex + 1) % 4;
    }

    public int RotationIndex => rotationIndex;
    public float RotationAngle => -90f * rotationIndex;

}
