using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class InventoryUI : MonoBehaviour
{
    public ItemToolTip itemToolTip;
    public GameObject rarePage;
    public GameObject bigPage;
    public GameObject smallPage;
    
    public ItemDataList_SO itemList;

    [Header("拖拽图片")]
    public Image dragItem;

    [Header("玩家背包UI")]
    [SerializeField] private GameObject bagUI;
    private bool bagOpened;

    [Header("通用背包")]
    [SerializeField] private GameObject baseBag;
    public GameObject shopSlotPrefab;

    [Header("图鉴")]
    public GameObject guideUI;
    public GameObject guideSlotPrefab;
    public GuideDetails guideDetails;

    [Header("交易UI")]
    public TradeUI tradeUI;
    public TextMeshProUGUI playerMoneyText;

    [SerializeField] private SlotUI[] playerSlots;
    [SerializeField] private List<SlotUI> baseBagSlots;
    [Header("祭坛")]
    public SlotUI[] altarSlots;
    public AltarManager altarDetails;
    private void OnEnable()
    {
        EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadedEvent;
        EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
        EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
        EventHandler.ShowTradeUI += OnShowTradeUI;
        EventHandler.ShowAltarUI += OnShowAltarUI;
    }

    private void OnDisable()
    {
        EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadedEvent;
        EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
        EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
        EventHandler.ShowTradeUI -= OnShowTradeUI;
        EventHandler.ShowAltarUI -= OnShowAltarUI;
    }

    private void OnShowAltarUI(InventoryLocation locationFrom, int fromIndex, InventoryLocation locationTarget, int targetIndex)
    {
        altarDetails.gameObject.SetActive(true);
        altarDetails.SetupAltarUI(locationFrom, fromIndex, locationTarget, targetIndex);
    }
    private void Awake()
    {
        foreach (ItemDetails item in itemList.itemDetailsList)
        {
            item.foundTimes = 1;
            if (item.itemType == ItemType.rareFish)
            {
                GuideUI guiSlot = Instantiate(guideSlotPrefab, rarePage.transform).GetComponent<GuideUI>();
                guiSlot.itemDetails = item;
            }
            if (item.itemType == ItemType.bigFish)
            {
                GuideUI guiSlot = Instantiate(guideSlotPrefab, bigPage.transform).GetComponent<GuideUI>();
                guiSlot.itemDetails = item;
            }
            if (item.itemType == ItemType.smallFish)
            {
                GuideUI guiSlot = Instantiate(guideSlotPrefab, smallPage.transform).GetComponent<GuideUI>();
                guiSlot.itemDetails = item;
            }
        }
    }


    private void Start()
    {
        //给每一个格子序号
        for (int i = 0; i < playerSlots.Length; i++)
        {
            playerSlots[i].slotIndex = i;
        }
        bagOpened = bagUI.activeInHierarchy;
        playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
        for (int i = 0; i < altarSlots.Length; i++)
        {
            altarSlots[i].slotIndex = i;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            OpenBagUI();
        }
    }

    private void OnShowTradeUI(ItemDetails item, bool isSell)
    {
        tradeUI.gameObject.SetActive(true);
        tradeUI.SetupTradeUI(item, isSell);
    }

    /// <summary>
    /// 打开通用包裹UI事件
    /// </summary>
    /// <param name="slotType"></param>
    /// <param name="bagData"></param>
    private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bagData)
    {
        GameObject prefab = slotType switch
        {
            SlotType.Shop => shopSlotPrefab,
            //SlotType.Box => boxSlotPrefab,
            _ => null,
        };

        //生成背包UI
        baseBag.SetActive(true);

        baseBagSlots = new List<SlotUI>();

        for (int i = 0; i < bagData.itemList.Count; i++)
        {
            SlotUI slot = Instantiate(prefab, baseBag.transform.GetChild(0)).GetComponent<SlotUI>();
            slot.slotIndex = i;
            baseBagSlots.Add(slot);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.GetComponent<RectTransform>());

        if (slotType == SlotType.Shop)
        {
            //bagUI.GetComponent<RectTransform>().pivot = new Vector2(-0.5f, 0.5f);
            bagUI.SetActive(true);
            bagOpened = true;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(bagUI.GetComponent<RectTransform>());
        //更新UI显示
        OnUpdateInventoryUI(InventoryLocation.Shop, bagData.itemList);
    }

    /// <summary>
    /// 关闭通用包裹UI事件
    /// </summary>
    /// <param name="slotType"></param>
    /// <param name="bagData"></param>
    private void OnBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bagData)
    {
        baseBag.SetActive(false);
        itemToolTip.gameObject.SetActive(false);
        UpdateSlotHightlight(-1);

        foreach (SlotUI slot in baseBagSlots)
        {
            Destroy(slot.gameObject);
        }
        baseBagSlots.Clear();

        if (slotType == SlotType.Shop)
        {
            bagUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            bagUI.SetActive(false);
            bagOpened = false;
        }
    }


    private void OnBeforeSceneUnloadedEvent()
    {
        UpdateSlotHightlight(-1);
    }


    /// <summary>
    /// 更新指定位置的Slot事件函数
    /// </summary>
    /// <param name="location">库存位置</param>
    /// <param name="list">数据列表</param>
    private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        switch (location)
        {
            case InventoryLocation.Player:
                for (int i = 0; i < playerSlots.Length; i++)
                {
                    if (list[i].itemAmount > 0)
                    {
                        ItemDetails item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                        playerSlots[i].UpdateSlot(item, list[i].itemAmount);
                    }
                    else
                    {
                        playerSlots[i].UpdateEmptySlot();
                    }
                }
                break;
            case InventoryLocation.Shop:
                for (int i = 0; i < baseBagSlots.Count; i++)
                {
                    if (list[i].itemAmount > 0)
                    {
                        ItemDetails item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                        baseBagSlots[i].UpdateSlot(item, list[i].itemAmount);
                    }
                    else
                    {
                        baseBagSlots[i].UpdateEmptySlot();
                    }
                }
                break;
            case InventoryLocation.Box:
                for (int i = 0; i < altarSlots.Length; i++)
                {
                    if (list[i].itemAmount > 0)
                    {
                        ItemDetails item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                        altarSlots[i].UpdateSlot(item, list[i].itemAmount);
                    }
                    else
                    {
                        altarSlots[i].UpdateEmptySlot();
                    }
                }
                break;
        }

        playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
    }

    /// <summary>
    /// 打开关闭背包UI，Button调用事件
    /// </summary>
    public void OpenBagUI()
    {
        bagOpened = !bagOpened;

        bagUI.SetActive(bagOpened);
    }


    /// <summary>
    /// 更新Slot高亮显示
    /// </summary>
    /// <param name="index">序号</param>
    public void UpdateSlotHightlight(int index)
    {
        foreach (SlotUI slot in playerSlots)
        {
            if (slot.isSelected && slot.slotIndex == index)
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

}
