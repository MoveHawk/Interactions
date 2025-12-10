using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item Data", fileName = "NewItemData")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public Sprite icon;

    [Header("Grid Size (2D Inventory Dimensions)")]
    [Min(1)] public int sizeX = 1;
    [Min(1)] public int sizeY = 1;

    [Header("Stacking")]
    public bool stackable = false;
    [Min(1)] public int maxStack = 1;

    [Header("Optional Description")]
    [TextArea] public string description;
}
