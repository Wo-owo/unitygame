using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AltarManager : MonoBehaviour
{
    private ItemDetails item;
    public Button submitButton;
    public Button cancelButton;

    private InventoryLocation locationFrom;
    private int fromIndex;
    private InventoryLocation locationTarget;
    private int targetIndex;

    public List<SlotUI> altarSlots=new List<SlotUI>();

    private void Awake()
    {
        submitButton.onClick.AddListener(TradeItem);
    }
    public void SetupAltarUI(InventoryLocation locationFrom, int fromIndex, InventoryLocation locationTarget, int targetIndex)
    {
        this.locationFrom = locationFrom;
        this.targetIndex = targetIndex;
        this.locationTarget = locationTarget;
        this.fromIndex = fromIndex;
    }
    private void TradeItem()
    {
        InventoryManager.Instance.SwapItem(locationFrom, fromIndex, locationTarget, targetIndex);

    }



    /// <summary>
    /// 开始献祭
    /// </summary>
    public void StartAltar(){
        Debug.Log("开始献祭");
        int _temp = 0;
        foreach(var _slot in altarSlots){
            if(_slot.itemDetails!=null){
                Debug.Log("有鱼");
                //if(_slot.itemDetails.itemType == ItemType.Fish){
                _temp++;
            }
        }
        if(_temp>10){
            GameManager.instance.fishingLevel = 3;
        }
        if(_temp>7){
            GameManager.instance.fishingLevel= 2;
        }
        if(_temp>3){
            GameManager.instance.fishingLevel = 1;
        }
        else{
            GameManager.instance.fishingLevel = 0;
        }
        GameManager.instance.baseLuck += _temp;

    }
}
