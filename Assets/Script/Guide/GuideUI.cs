using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class GuideUI : MonoBehaviour
{
    public Image slotImage;
    public Button button;
    public ItemDetails itemDetails;
    public ItemToolTip guideDetails;

    private void Awake()
    {
        slotImage.sprite = itemDetails.itemIcon;
        button.interactable = itemDetails.isFound;
        //guideDetails = GameObject.Find("GuideDetails").GetComponent<ItemToolTip>();
    }
    private void Update()
    {
        if (itemDetails.isFound == true)
        {
            slotImage.sprite = itemDetails.itemIcon;
            button.interactable =true;
        }
        else
        {
            slotImage.sprite = null;
            button.interactable = false;
        }
    }
    public void OpenGuideDetails()
    {
        guideDetails.SetupTooltip(itemDetails,SlotType.Box);
        guideDetails.gameObject.SetActive(true);
    }

}
