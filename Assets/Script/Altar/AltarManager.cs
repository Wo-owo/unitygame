using System;
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
}
