using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using static UnityEditor.Progress;

public enum MiniResultType
{
    异常 = -1,
    普通成功,
    完美钓起,
    钓鱼失败
}
public class MiniGameManager : Singleton<MiniGameManager>
{
    public ProgressBar mProgressBar;
    public FishMove Fish;
    public CatchFish CatchFish;
    public KeyCode Catch_MoveUP_Key;
    public float Catch_Move_Speed;
    public float time;
    public float DescTimeMax;
    public int fishID;
    public bool IsStart = false;
    public MiniResultType MiniResult;
    private Canvas canvas;
    //TODO暂时通过button按钮调用默认1001物品
    protected override void Awake()
    {
        base.Awake();
        canvas = transform.parent.GetComponent<Canvas>();
    }
    public void StartGame(int ID)
    {
        IsStart = true;
        fishID = ID;
        time = 0;
        mProgressBar.Size = 0;
        CatchFish.LeaveCount = 0;
        Fish.transform.Translate(new Vector2(0, Random.Range(-100, 100)));
        CatchFish.transform.Translate(new Vector2(0, Random.Range(-100, 100)));
        canvas.targetDisplay = 0;
    }

    private void Update()
    {
        if (!IsStart)
            return;
        time += Time.deltaTime;
        if (time > DescTimeMax && mProgressBar.Size < 1)
        {
            canvas.targetDisplay = 1;
            MiniResult = MiniResultType.钓鱼失败;
            IsStart = false;
            return;
        }
        if (mProgressBar.Size >= 1)
        {
            IsStart = false;
            if (CatchFish.LeaveCount > 0)
            {
                canvas.targetDisplay = 1;
                MiniResult = MiniResultType.普通成功;
            }
            else
            {
                canvas.targetDisplay = 1;
                MiniResult = MiniResultType.完美钓起;
            }
            return;
        }
        if (Input.GetKey(Catch_MoveUP_Key))
        {
            CatchFish.transform.Translate(Catch_Move_Speed * Time.deltaTime * Vector2.up);
        }
        else
        {
            CatchFish.transform.Translate(Catch_Move_Speed * Time.deltaTime * Vector2.down);
        }
    }
    public void SetProgressBarValue(float value)
    {
        mProgressBar.Size += value;
    }

}
