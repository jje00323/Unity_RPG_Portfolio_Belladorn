using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Rune,
    Consumable,
    Material,
    Quest,
    Etc
}
public enum ItemRarity { Normal, Rare, Epic }

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("�⺻ ����")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public ItemType itemType;

    [Header("���")]
    public ItemRarity rarity;

    [Header("���� ����")]
    public bool isStackable = false;
    public int maxStack = 1;
}