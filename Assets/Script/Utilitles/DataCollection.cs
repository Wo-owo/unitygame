using UnityEngine;

[System.Serializable]

public class ItemDetails
{
    public int itemID;

    public string itemName;

    public Sprite itemIcon;
    public Sprite itemOnWorldSprite;
    //��Ʒ����
    public ItemType itemType;

    public string itemDescription;

    public int itemPrice;
    [Range(0, 1)]
    public float sellPercentage;

    public int itemLucky;
    //�;ö�
    public float itemDurability;

    public float itemWeight;
    public float maxWeight;

    //�Ƿ������
    public bool isFound;
}
[System.Serializable]
public struct InventoryItem
{
    public int itemID;
    public int itemAmount;
}