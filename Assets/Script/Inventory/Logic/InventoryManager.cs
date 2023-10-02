using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [Header("物品数据")]
    public ItemDataList_SO itemDataList_SO;
    [Header("背包数据")]
    public InventoryBag_SO playerBag;
    public InventoryBag_SO altarList;

    [Header("交易")]
    public int playerMoney;

    private void Start()
    {
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Box, altarList.itemList);
        //var sprite = Resources.LoadAll<Sprite>($"Sprite/Fish/普通鱼");
        //for (int i = 0; i < sprite.Length; i++)
        //{
        //    var item = new ItemDetails();
        //    item.itemName = sprite[i].name;
        //    item.itemID = 1016 + i;
        //    item.itemWeight = 1.5f;
        //    item.itemIcon = sprite[i];
        //    item.itemOnWorldSprite = sprite[i];
        //    itemDataList_SO.itemDetailsList.Add(item);
        //}
    }


    /// <summary>
    /// 通过ID返回物品信息
    /// </summary>
    /// <param name="ID">Item ID</param>
    /// <returns></returns>
    public ItemDetails GetItemDetails(int ID)
    {
        return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
    }

    /// <summary>
    /// 添加物品到Player背包里
    /// </summary>
    /// <param name="item"></param>
    /// <param name="toDestory">是否要销毁物品</param>
    public void AddItem(Item item, bool toDestory)
    {
        //是否已经有该物品
        var index = GetItemIndexInBag(item.itemID);

        AddItemAtIndex(item.itemID, index, 1);

        //Debug.Log(GetItemDetails(item.itemID).itemID + "Name: " + GetItemDetails(item.itemID).itemName);
        if (toDestory)
        {
            Destroy(item.gameObject);
        }

        //更新UI
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
    }
    /// <summary>
    /// 通过物品ID添加物品
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="itemCount"></param>
    /// <param name="toDestory"></param>
    public void AddItem(int itemId, int itemCount)
    {
        //是否已经有该物品
        int index = GetItemIndexInBag(itemId);

        AddItemAtIndex(itemId, index, itemCount);

        //Debug.Log(GetItemDetails(item.itemID).itemID + "Name: " + GetItemDetails(item.itemID).itemName);

        //更新UI
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
    }


    /// <summary>
    /// 检查背包是否有空位
    /// </summary>
    /// <returns></returns>
    private bool CheckBagCapacity()
    {
        for (int i = 0; i < playerBag.itemList.Count; i++)
        {
            if (playerBag.itemList[i].itemID == 0)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 通过物品ID找到背包已有物品位置
    /// </summary>
    /// <param name="ID">物品ID</param>
    /// <returns>-1则没有这个物品否则返回序号</returns>
    public int GetItemIndexInBag(int ID)
    {
        for (int i = 0; i < playerBag.itemList.Count; i++)
        {
            if (playerBag.itemList[i].itemID == ID)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// 在指定背包序号位置添加物品
    /// </summary>
    /// <param name="ID">物品ID</param>
    /// <param name="index">序号</param>
    /// <param name="amount">数量</param>
    private void AddItemAtIndex(int ID, int index, int amount)
    {
        if (index == -1 && CheckBagCapacity())    //背包没有这个物品 同时背包有空位
        {
            var item = new InventoryItem { itemID = ID, itemAmount = amount };
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == 0)
                {
                    playerBag.itemList[i] = item;
                    break;
                }
            }
        }
        else    //背包有这个物品
        {
            int currentAmount = playerBag.itemList[index].itemAmount + amount;
            var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };

            playerBag.itemList[index] = item;
        }
    }

    /// <summary>
    /// Player背包范围内交换物品
    /// </summary>
    /// <param name="fromIndex">起始序号</param>
    /// <param name="targetIndex">目标数据序号</param>
    public void SwapItem(int fromIndex, int targetIndex)
    {
        InventoryItem currentItem = playerBag.itemList[fromIndex];
        InventoryItem targetItem = playerBag.itemList[targetIndex];

        if (targetItem.itemID != 0)
        {
            playerBag.itemList[fromIndex] = targetItem;
            playerBag.itemList[targetIndex] = currentItem;
        }
        else
        {
            playerBag.itemList[targetIndex] = currentItem;
            playerBag.itemList[fromIndex] = new InventoryItem();
        }

        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
    }

    /// <summary>
    /// 跨背包交换数据
    /// </summary>
    /// <param name="locationFrom"></param>
    /// <param name="fromIndex"></param>
    /// <param name="locationTarget"></param>
    /// <param name="targetIndex"></param>
    public void SwapItem(InventoryLocation locationFrom, int fromIndex, InventoryLocation locationTarget, int targetIndex)
    {

        List<InventoryItem> currentList = GetItemList(locationFrom);
        List<InventoryItem> targetList = GetItemList(locationTarget);

        InventoryItem currentItem = currentList[fromIndex];

        if (targetIndex < targetList.Count)
        {
            InventoryItem targetItem = targetList[targetIndex];

            if (targetItem.itemID != 0 && currentItem.itemID != targetItem.itemID)  //有不相同的两个物品
            {
                currentList[fromIndex] = targetItem;
                targetList[targetIndex] = currentItem;
            }
            else if (currentItem.itemID == targetItem.itemID) //相同的两个物品
            {
                targetItem.itemAmount += currentItem.itemAmount;
                targetList[targetIndex] = targetItem;
                currentList[fromIndex] = new InventoryItem();
            }
            else    //目标空格子
            {
                targetList[targetIndex] = currentItem;
                currentList[fromIndex] = new InventoryItem();
            }
            EventHandler.CallUpdateInventoryUI(locationFrom, currentList);
            EventHandler.CallUpdateInventoryUI(locationTarget, targetList);
        }
    }

    /// <summary>
    /// 根据位置返回背包数据列表
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    private List<InventoryItem> GetItemList(InventoryLocation location)
    {
        return location switch
        {
            InventoryLocation.Player => playerBag.itemList,
            InventoryLocation.Box => altarList.itemList,
            _ => null
        };
    }

    /// <summary>
    /// 移除指定数量的背包物品
    /// </summary>
    /// <param name="ID">物品ID</param>
    /// <param name="removeAmount">数量</param>
    private void RemoveItem(int ID, int removeAmount)
    {
        var index = GetItemIndexInBag(ID);

        if (playerBag.itemList[index].itemAmount > removeAmount)
        {
            var amount = playerBag.itemList[index].itemAmount - removeAmount;
            var item = new InventoryItem { itemID = ID, itemAmount = amount };
            playerBag.itemList[index] = item;
        }
        else if (playerBag.itemList[index].itemAmount == removeAmount)
        {
            var item = new InventoryItem();
            playerBag.itemList[index] = item;
        }

        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
    }


    /// <summary>
    /// 交易物品
    /// </summary>
    /// <param name="itemDetails">物品信息</param>
    /// <param name="amount">交易数量</param>
    /// <param name="isSellTrade">是否卖东西</param>
    public void TradeItem(ItemDetails itemDetails, int amount, bool isSellTrade)
    {
        int cost = itemDetails.itemPrice * amount;
        //获得物品背包位置
        int index = GetItemIndexInBag(itemDetails.itemID);

        if (isSellTrade)    //卖
        {
            if (playerBag.itemList[index].itemAmount >= amount)
            {
                RemoveItem(itemDetails.itemID, amount);
                //卖出总价
                cost = (int)(cost * itemDetails.sellPercentage);
                playerMoney += cost;
            }
        }
        else if (playerMoney - cost >= 0)   //买
        {
            if (CheckBagCapacity())
            {
                AddItemAtIndex(itemDetails.itemID, index, amount);
            }
            playerMoney -= cost;
        }
        //刷新UI
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
    }




}