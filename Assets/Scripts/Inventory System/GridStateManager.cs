using UnityEngine;

public class GridStateManager : MonoBehaviour
{
    
    [Header("Grid Dimension")]
    public int gridWidth; //Columns
    public int gridHeight; //Rows

    private InventoryItem[,] gridMatrix;

    public InventoryItem GetItem(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight) return null;
        return gridMatrix[x, y];
    }


    private void Awake()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        if (gridWidth > 0 && gridHeight > 0)
        {
            gridMatrix = new InventoryItem[gridWidth, gridHeight];
        }
    }

    public bool IsWithinBounds(int startX, int startY, int width, int height)
    {
        if (startX < 0 || startY < 0) return false;
        if ((startX + width) > gridWidth) return false;
        if ((startY + height) > gridHeight)return false;

        return true;
    }

    public bool IsSpaceAvailable(int startX, int startY, int width, int height)
    {
        for (int x = startX; x < startX + width; x++)
        {
            for (int y = startY; y < startY + height; y++)
            {
                if (gridMatrix[x, y] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool PlaceItem(InventoryItem item, int startX, int startY)
    {
        int width = item.GetWidth();
        int height = item.GetHeight();

        if (startX < 0 || startY < 0 || startX + width > gridWidth || startY + height > gridHeight)
        {
            return false;
        }

        for (int x = startX; x < startX + width; x++)
        {
            for (int y = startY; y < startY + height; y++)
            {
                if (gridMatrix[x, y] != null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void RemoveItem(InventoryItem item)
    {
        int startX = item.originPosition.x;
        int startY = item.originPosition.y;
        int width = item.GetWidth();
        int height = item.GetHeight();

        for (int x = startX; x < startX + width; x++)
        {
            for (int y = startY; y < startY + height; y++)
            {
                if (gridMatrix[x, y] == item)
                {
                    gridMatrix[x, y] = null;
                }
            }
        }
    }
}
