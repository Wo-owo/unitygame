using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMove : MonoBehaviour
{
    float yMax;
    float yMin;
    private void Start()
    {
        MiniGameManager.Instance.Fish = this;
        var size = transform.parent.GetComponent<RectTransform>().sizeDelta;
        var mHeight = transform.GetComponent<RectTransform>().sizeDelta.y / 2;
        yMax = size.y / 2 - mHeight;
        yMin = -size.y / 2 + mHeight;
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector2(0, Random.Range(-100, 100)) * Time.deltaTime);
        if (transform.localPosition.y > 150)
        {
            transform.localPosition = new Vector2(0, 150);
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
