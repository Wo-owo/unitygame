using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Text moneyText;
    [SerializeField] private Text weightText;
    [SerializeField] private GameObject weightPart;

    public void SetupTooltip(ItemDetails itemDetails,SlotType slotType)
    {
        nameText.text = itemDetails.itemName;
        typeText.text = GetItemType(itemDetails.itemType);
        descriptionText.text = itemDetails.itemDescription;

        if (itemDetails.itemType == ItemType.Fish || itemDetails.itemType == ItemType.FishingRod || itemDetails.itemType == ItemType.Bait)
        {
            //weightPart.SetActive(true);
            int price = itemDetails.itemPrice;
            float weight=itemDetails.itemWeight;
            if (slotType == SlotType.Bag)
            {
                price = (int)(price * itemDetails.sellPercentage);
                
            }

            moneyText.text = price.ToString();
            weightText.text = "重量:"+weight.ToString()+"kg";
        }
        else
        {
            weightPart.SetActive(false);
        }
        if(itemDetails.itemType != ItemType.Fish)
        {
            weightPart.SetActive(false);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
    private string GetItemType(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Fish => "鱼类",
            ItemType.Bait => "饵料",
            ItemType.FishingRod => "钓竿",
            _ => "无"
        };
    }
}
