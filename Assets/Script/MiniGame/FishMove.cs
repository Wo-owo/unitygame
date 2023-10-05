using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMove : MonoBehaviour
{
    float yMax;
    float yMin;
    public float MoveSpeed;
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
        if (!MiniGameManager.Instance.IsStart)
            return;
        if (!OnMove)
        {
            StartCoroutine(Move());
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
    bool OnMove;
    IEnumerator Move()
    {
        OnMove = true;
        var vec = new Vector3(0, Random.Range(yMin, yMax));
        while (Vector3.Distance(transform.localPosition, vec) >= 0.1)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, vec, 0.01f);
            yield return null;
        }
        OnMove = false;
    }
}
