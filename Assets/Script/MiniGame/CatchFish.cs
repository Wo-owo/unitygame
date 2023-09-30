using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CatchFish : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Fish = collision;
    }
    public int LeaveCount;
    private void OnTriggerExit2D(Collider2D collision)
    {
        Fish = null;
        LeaveCount++;
    }
    public Collider2D Fish;
    public float CathcFish_AddValue;
    public float CathcFish_DecValue;
    public event UnityAction<float> FishChanged;
    float yMax;
    float yMin;
    private void Start()
    {
        MiniGameManager.Instance.CatchFish = this;
        FishChanged += MiniGameManager.Instance.SetProgressBarValue;
        var size = transform.parent.GetComponent<RectTransform>().sizeDelta;
        var mHeight = transform.GetComponent<RectTransform>().sizeDelta.y / 2;
        yMax = size.y / 2 - mHeight;
        yMin = -size.y / 2 + mHeight;
    }
    void Update()
    {
        if (!MiniGameManager.Instance.IsStart)
            return;
        if (Fish == null)
        {
            FishChanged?.Invoke(CathcFish_DecValue);
        }
        else
        {
            FishChanged?.Invoke(CathcFish_AddValue);
        }
        if (transform.localPosition.y < yMin)
        {
            transform.localPosition = new Vector2(0, yMin);
        }
        if (transform.localPosition.y > yMax)
        {
            transform.localPosition = new Vector2(0, yMax);
        }
    }
}
