using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggrItemFader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemFader[] faders = collision.GetComponentsInChildren<ItemFader>();
        if(faders.Length > 0)
        {
            foreach(ItemFader item in faders)
            {
                item.FadOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ItemFader[] faders = collision.GetComponentsInChildren<ItemFader>();
        if (faders.Length > 0)
        {
            foreach (ItemFader item in faders)
            {
                item.FadIn();
            }
        }
    }
}
