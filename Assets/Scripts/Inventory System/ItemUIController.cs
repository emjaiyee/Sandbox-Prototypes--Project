using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Numerics;

public class ItemUIController : MonoBehaviour
{

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI stackText;
    [SerializeField] private RectTransform rectTransform;

    public void Setup(InventoryItem item, float cellSize)
    {
        iconImage.sprite = item.itemData.icon;

        float width = item.itemData.gridWidth * cellSize;
        float height = item.itemData.gridHeight * cellSize;
        rectTransform.sizeDelta = new UnityEngine.Vector2(width, height);

        if (stackText != null)
        {
            stackText.gameObject.SetActive(true);
            stackText.text = item.quantity.ToString();
        }

        if (iconImage != null)
        {
            iconImage.sprite = item.itemData.icon;
        }

        UpdateLayout(item, cellSize);
    }

    public void UpdateLayout(InventoryItem item, float cellSize)
    {
        float activeWidth = item.GetWidth() * cellSize;
        float activeHeight = item.GetHeight() * cellSize;
        rectTransform.sizeDelta = new UnityEngine.Vector2(activeWidth, activeHeight);

        if (iconImage != null)
        {
            float unrotatedWidth = item.itemData.gridWidth * cellSize;
            float unrotatedHeight = item.itemData.gridHeight * cellSize;
            iconImage.rectTransform.sizeDelta = new UnityEngine.Vector2(unrotatedWidth, unrotatedHeight);

            iconImage.rectTransform.anchoredPosition = new UnityEngine.Vector2(activeWidth / 2f, -activeHeight / 2f);

            iconImage.rectTransform.localEulerAngles = new UnityEngine.Vector3(0, 0, item.RotationAngle);

            if (stackText != null)
            {
                stackText.gameObject.SetActive(true);
                stackText.text = item.quantity.ToString();
            }
        }
    }
}
