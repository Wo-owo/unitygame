using UnityEngine;

[System.Serializable]

public class ItemDetails
{
    public int itemID;

    public string itemName;

    public Sprite itemIcon;
    public Sprite itemOnWorldSprite;
    //物品类型
    public ItemType itemType;

    public string itemDescription;

    public int itemPrice;
    [Range(0, 1)]
    public float sellPercentage;


    //耐久度
    public float itemDurability;

    public float itemWeight;
    public float maxWeight;
    public float minWeight;
    public int itemChance;//概率
    public int rareDegree;//稀有程度
    
    public Habitat habitat;//栖息地
    //是否钓到过
    public int foundTimes;
}
[System.Serializable]
public struct InventoryItem
{
    public int itemID;
    public int itemAmount;
}