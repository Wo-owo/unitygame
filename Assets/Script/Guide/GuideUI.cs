using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class GuideUI : MonoBehaviour
{
    public Image slotImage;
    public Button button;
    public ItemDetails itemDetails;
    public Sprite nullImage;
    private void Awake()
    {
        slotImage.sprite = itemDetails.itemIcon;
        button.interactable = itemDetails.foundTimes>0?true:false;
    }
    private void Update()
    {
        if (itemDetails.foundTimes > 0)
        {
            slotImage.sprite = itemDetails.itemIcon;
            button.interactable = true;
        }
        else
        {
            slotImage.sprite = nullImage;
            button.interactable = false;
        }
    }

}
