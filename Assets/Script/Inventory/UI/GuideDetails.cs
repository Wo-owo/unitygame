using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GuideDetails : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Text moneyText;
    [SerializeField] private Text weightText;
    [SerializeField] private GameObject weightPart;
    [SerializeField] private Text maxWeight;
    [SerializeField] private Text times;
    [SerializeField] private Image itemIcon;

    public void SetupTooltip(ItemDetails itemDetails, SlotType slotType)
    {
        if (itemDetails == null) return;
        nameText.text = itemDetails.itemName;
        typeText.text = GetItemType(itemDetails.itemType);
        descriptionText.text = itemDetails.itemDescription;

        //weightPart.SetActive(true);
        int price = itemDetails.itemPrice;
        float weight = itemDetails.itemWeight;
        if (slotType == SlotType.Bag)
        {
            price = (int)(price * itemDetails.sellPercentage);

        }
            moneyText.text = price.ToString();
        weightText.text = weight.ToString() + "kg";
        times.text = itemDetails.foundTimes.ToString() + "¥Œ";
        maxWeight.text = itemDetails.maxWeight.ToString() + "kg";
        itemIcon.sprite = itemDetails.itemIcon;
            weightPart.SetActive(false);
        if (itemDetails.itemType != ItemType.Fish)
        {
            weightPart.SetActive(false);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
    private string GetItemType(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Fish => "”„¿‡",
            ItemType.rareFish=>"’‰œ°”„",
            ItemType.Bait => "∂¸¡œ",
            ItemType.FishingRod => "µˆ∏Õ",
            _ => "”„¿‡"
        };
    }
}
