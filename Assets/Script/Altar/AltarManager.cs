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
        cancelButton.onClick.AddListener(CancelUI);
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

        CancelUI();
    }


    private void CancelUI()
    {
        this.gameObject.SetActive(false);
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
                
                    if(_slot.itemDetails.itemType == ItemType.smallFish){
                        _temp +=1;
                    }
                    else if(_slot.itemDetails.itemType == ItemType.bigFish){
                        _temp +=5;
                    }
                    else if(_slot.itemDetails.itemType == ItemType.rareFish){
                        _temp +=10;
                    }
                    
                //}
                else if(_slot.itemDetails==null){
                
                }
            }
            
        }
        GameManager.instance.baseLuck += _temp;

    }
}
