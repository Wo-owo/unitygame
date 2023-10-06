using UnityEngine;

public class Raindrop : MonoBehaviour
{
    public float fallSpeed = 5f;
    public float resetTime = 3f;

    private Vector3 initialPosition;
    private bool isFalling = false;

    void Start()
    {
        initialPosition = transform.position;
        StartFalling();
    }

    void Update()
    {
        if (isFalling)
        {
            // 移动雨滴向下
            transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

            // 如果雨滴到达屏幕底部，将其重新定位到屏幕顶部并停止下落
            if (transform.position.y < -Camera.main.orthographicSize)
            {
                ResetPosition();
            }
        }
    }

    void ResetPosition()
    {
        // 重新定位雨滴到初始位置
        transform.position = initialPosition;
        isFalling = false;

        // 启动计时器，到达resetTime后开始下落
        Invoke("StartFalling", resetTime);
    }

    void StartFalling()
    {
        isFalling = true;
    }
}
