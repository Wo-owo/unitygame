using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("物品数据")]
        public ItemDataList_SO itemDataList_SO;
        [Header("背包数据")]
        public InventoryBag_SO playerBag;


        private void OnEnable()
        {
            EventHeadler.DropItemEvent += OnDropItemEvent;
            EventHeadler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;
            //建造
        //    EventHeadler.BuildFurnitureEvent += OnBuildFurnitureEvent;
        //    EventHeadler.BaseBagOpenEvent += OnBaseBagOpenEvent;
        //    EventHeadler.StartNewGameEvent += OnStartNewGameEvent;
        }


        private void OnDisable()
        {
            EventHeadler.DropItemEvent -= OnDropItemEvent;
            EventHeadler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;
            //EventHeadler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
            //EventHeadler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            //EventHeadler.StartNewGameEvent -= OnStartNewGameEvent;
        }

        private void Start()
        {
            EventHeadler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        private void OnDropItemEvent(int ID, Vector3 Pos, ItemType type)
        {
            RemoveItem(ID, 1);
        }

        /// <summary>
        /// 通过ID返回物品信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
        }
        /// <summary>
        /// 添加物品到Player背包中
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestory">是否摧毁物品</param>
        public void AddItem(Item item, bool toDestory)
        {
            //是否有物品
            int index = GetItemIndexInBag(item.itemID);

            //是否有空位
            AddItemAtIndex(item.itemID, index, 1);


            Debug.Log(GetItemDetails(item.itemID).itemID + "Name:" + GetItemDetails(item.itemID).itemName);
            if (toDestory)
            {
                Destroy(item.gameObject);
            }

            //更新UI
            EventHeadler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        private void OnHarvestAtPlayerPosition(int ID)
        {
            //是否已经有该物品
            int index = GetItemIndexInBag(ID);

            AddItemAtIndex(ID, index, 1);

            //更新UI
            EventHeadler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        /// <summary>
        /// 检测背包是否有空位
        /// </summary>
        /// <returns></returns>
        private bool CheckBagCapacity()
        {
            for(int i=0;i<playerBag.itemList.Count;i++)
            {
                if (playerBag.itemList[i].itemID == 0)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 通过物品ID找到已有物品的位置
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        private int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// 在指定位置添加物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="index">序号</param>
        /// <param name="amount">数量</param>
        private void AddItemAtIndex(int ID,int index,int amount)
        {
            if (index == -1 && CheckBagCapacity())
            {
                InventoryItem item = new InventoryItem { itemID = ID, itemAmount = amount };
                for (int i = 0; i < playerBag.itemList.Count; i++)
                {
                    if (playerBag.itemList[i].itemID == 0)
                    {
                        playerBag.itemList[i] = item;
                        break;
                    }
                }
            }
            else
            {
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                InventoryItem item = new InventoryItem { itemID = ID, itemAmount = currentAmount };

                playerBag.itemList[index] = item;
            }
        }
        /// <summary>
        /// player背包范围内交换物品
        /// </summary>
        /// <param name="fromIndex">起始序号</param>
        /// <param name="targetIndex">终止序号</param>
        public void SwapItem(int fromIndex,int targetIndex)
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
            EventHeadler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        /// <summary>
        /// 删除指定数量的背包物品
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="removeAmount"></param>
        private void RemoveItem(int ID,int removeAmount)
        {
            int index = GetItemIndexInBag(ID);

            if (playerBag.itemList[index].itemAmount > removeAmount)
            {
                int amount = playerBag.itemList[index].itemAmount - removeAmount;
                InventoryItem item= new InventoryItem { itemID= ID, itemAmount = amount };
                playerBag.itemList[index] = item;
            }
            else if (playerBag.itemList[index].itemAmount == removeAmount)
            {
                InventoryItem item = new InventoryItem();
                playerBag.itemList[index] = item;
            }

            EventHeadler.CallUpdateInventoryUI(InventoryLocation.Player,playerBag.itemList);
        }
    }
    }
