using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MFarm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        public ItemToolTip itemToolTip;
        [Header("拖拽图片")]
        public Image dragItem;
        [Header("玩家背包UI")]
        [SerializeField] private GameObject bagUI;
        private bool bagOpended;
        [SerializeField] private SlotUI[] playerSlots;



        private void OnEnable()
        {
            EventHeadler.UpdateInventoryUI += OnUpdateInventoryUI;
            EventHeadler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        }

        private void OnDisable()
        {
            EventHeadler.UpdateInventoryUI -= OnUpdateInventoryUI;
            EventHeadler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        }

       

        private void Start()
        {
            for(int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].slotIndex = i;
            }
            bagOpended = bagUI.activeInHierarchy;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                OpenBagUI();
            }
        }
        private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
        {
            switch (location)
            {
                case InventoryLocation.Player:
                    for (int i = 0; i < playerSlots.Length; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            playerSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            playerSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            //    case InventoryLocation.Box:
            //        for (int i = 0; i < baseBagSlots.Count; i++)
            //        {
            //            if (list[i].itemAmount > 0)
            //            {
            //                var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
            //                baseBagSlots[i].UpdateSlot(item, list[i].itemAmount);
            //            }
            //            else
            //            {
            //                baseBagSlots[i].UpdateEmptySlot();
            //            }
            //        }
            //        break;
            }

            //playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
        }
        /// <summary>
        /// 打开关闭背包UI,Button调用事件
        /// </summary>
        public void OpenBagUI()
        {
            bagOpended = !bagOpended;
            bagUI.SetActive(bagOpended);
        }
        /// <summary>
        /// 更新高亮显示
        /// </summary>
        /// <param name="index">序号</param>
        public void UpdateSlotHightlight(int index)
        {
            foreach(SlotUI slot in playerSlots)
            {
                if(slot.isSelected&&slot.slotIndex == index)
                {
                    slot.slotHightlight.gameObject.SetActive(true);
                }
                else
                {
                    slot.isSelected = false;
                    slot.slotHightlight.gameObject.SetActive(false);
                }
            }
        }
        private void OnBeforeSceneUnloadEvent()
        {
            UpdateSlotHightlight(-1);
        }
    }
}
