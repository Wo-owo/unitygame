using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MFarm.Inventory
{
    public class SlotUI : MonoBehaviour,IPointerClickHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        [Header("�����ȡ")]
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] public Image slotHightlight;
        [SerializeField] private Button button;
        [Header("��������")]
        public SlotType slotType;
        public bool isSelected;
        public ItemDetails itemDetails;
        public int itemAmount;
        public int slotIndex;

        public InventoryUI inventoryUI=>GetComponentInParent<InventoryUI>();

        private void Start()
        {
            isSelected = false;

            if (itemDetails == null)
            {
                UpdateEmptySlot();
            }
        }
        /// <summary>
        /// ���¸���UI����Ϣ
        /// </summary>
        /// <param name="item">ItmDetails</param>
        /// <param name="amount">��������</param>
        public void UpdateSlot(ItemDetails item, int amount)
        {
            itemDetails = item;
            slotImage.sprite = item.itemIcon;
            itemAmount = amount;
            amountText.text = amount.ToString();
            slotImage.enabled = true;
            button.interactable = true;
        }

        /// <summary>
        /// ��slot����Ϊ��
        /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;

                inventoryUI.UpdateSlotHightlight(-1);
                EventHeadler.CallItemSelectedEvent(itemDetails, isSelected);
            }
            itemDetails = null;
            slotImage.enabled = false;
            amountText.text = string.Empty;
            button.interactable = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemDetails == null) return;
            isSelected = !isSelected;

            inventoryUI.UpdateSlotHightlight(slotIndex);

            if (slotType == SlotType.Bag)
            {
                //֪ͨ��Ʒ��ѡ�е�״̬����Ϣ
                EventHeadler.CallItemSelectedEvent(itemDetails, isSelected);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemAmount != 0)
            {
                inventoryUI.dragItem.enabled = true;
                inventoryUI.dragItem.sprite = slotImage.sprite;
                inventoryUI.dragItem.SetNativeSize();

                isSelected = true;
                inventoryUI.UpdateSlotHightlight(slotIndex);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.enabled= false;

            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null)
                    return;
                SlotUI targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                int targetIndex=targetSlot.slotIndex;

                //�����ڽ�����Ʒ
                if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
                }
                //��ո�����ʾ
                inventoryUI.UpdateSlotHightlight(-1);
            }
            //else
            //{
            //    if (itemDetails.canDropped)
            //    {
            //        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

            //        EventHeadler.CallInstantiateItemInScene(itemDetails.itemID, pos);
            //    }
            //}
        }
    }
}