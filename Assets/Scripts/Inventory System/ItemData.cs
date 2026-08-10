using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{

    public string itemName;

    public Sprite icon;
    [TextArea] public string itemDescription;

    [Header("Item Size")]
    public int gridWidth;
    public int gridHeight;

    
}
