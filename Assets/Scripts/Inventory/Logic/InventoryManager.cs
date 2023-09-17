using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("��Ʒ����")]
        public ItemDataList_SO itemDataList_SO;
        [Header("��������")]
        public InventoryBag_SO playerBag;


        private void OnEnable()
        {
            EventHeadler.DropItemEvent += OnDropItemEvent;
            EventHeadler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;
            //����
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
        /// ͨ��ID������Ʒ��Ϣ
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
        }
        /// <summary>
        /// �����Ʒ��Player������
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestory">�Ƿ�ݻ���Ʒ</param>
        public void AddItem(Item item, bool toDestory)
        {
            //�Ƿ�����Ʒ
            int index = GetItemIndexInBag(item.itemID);

            //�Ƿ��п�λ
            AddItemAtIndex(item.itemID, index, 1);


            Debug.Log(GetItemDetails(item.itemID).itemID + "Name:" + GetItemDetails(item.itemID).itemName);
            if (toDestory)
            {
                Destroy(item.gameObject);
            }

            //����UI
            EventHeadler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        private void OnHarvestAtPlayerPosition(int ID)
        {
            //�Ƿ��Ѿ��и���Ʒ
            int index = GetItemIndexInBag(ID);

            AddItemAtIndex(ID, index, 1);

            //����UI
            EventHeadler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        /// <summary>
        /// ��ⱳ���Ƿ��п�λ
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
        /// ͨ����ƷID�ҵ�������Ʒ��λ��
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
        /// ��ָ��λ�������Ʒ
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <param name="index">���</param>
        /// <param name="amount">����</param>
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
        /// player������Χ�ڽ�����Ʒ
        /// </summary>
        /// <param name="fromIndex">��ʼ���</param>
        /// <param name="targetIndex">��ֹ���</param>
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
        /// ɾ��ָ�������ı�����Ʒ
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
