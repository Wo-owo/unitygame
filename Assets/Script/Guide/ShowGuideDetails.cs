using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGuideDetails : MonoBehaviour
{
    private GuideUI guideUI;
    private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

    private void Awake()
    {
        guideUI = GetComponent<GuideUI>();
    }

    public void OnpenDuideDatails()
    {
        inventoryUI.guideDetails.gameObject.SetActive(true);
        inventoryUI.guideDetails.SetupTooltip(guideUI.itemDetails, SlotType.Box);
    }
}
