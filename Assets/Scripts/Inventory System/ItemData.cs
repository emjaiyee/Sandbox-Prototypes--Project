using UnityEngine;



public enum EquipmentType
{
    None,
    Consumable,
    Weapon,
    Helmet,
    Chestplate,
    Legging
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{

    [Header("Item Type")]
    [SerializeField] private EquipmentType equipmentType = EquipmentType.None;
    public EquipmentType EquipmentType => equipmentType;

    public string itemName;
    public Sprite icon;
    [TextArea] public string itemDescription;

    [Header("Item Size")]
    public int gridWidth = 1;
    public int gridHeight = 1;

    [Header("Stacking")]
    public bool isStackable = false;
    public int maxStackSize = 1;
}