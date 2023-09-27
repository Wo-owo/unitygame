using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameManager : Singleton<MiniGameManager>
{
    public ProgressBar mProgressBar;
    public FishMove Fish;
    public CatchFish CatchFish;
    public KeyCode Catch_MoveUP_Key;
    public float Catch_Move_Speed;
    public float time;
    public float DescTimeMax;
    public void StartGame()
    {
        Fish.enabled = true;
        CatchFish.enabled = true;
        this.enabled = true;
        time = 0;
        Fish.transform.Translate(new Vector2(0, Random.Range(-100, 100)));
        CatchFish.transform.Translate(new Vector2(0, Random.Range(-100, 100)));
    }
    private void Update()
    {
        time += Time.deltaTime;
        if (time > DescTimeMax && mProgressBar.Size < 1)
        {
            Fish.enabled = false;
            CatchFish.enabled = false;
            this.enabled = false;
            Debug.Log("失败");
            return;
        }
        if (mProgressBar.Size >= 1)
        {
            Fish.enabled = false;
            CatchFish.enabled = false;
            this.enabled = false;
            if (CatchFish.LeaveCount > 0)
            {
                Debug.Log("成功");
            }
            else
            {
                Debug.Log("完美");
            }
            return;
        }
        if (Input.GetKey(Catch_MoveUP_Key))
        {
            CatchFish.transform.Translate(Catch_Move_Speed * Time.deltaTime * Vector2.up);
        }
        else
        {
            CatchFish.transform.Translate(Catch_Move_Speed * Time.deltaTime * -Vector2.up);
        }
    }
    public void SetProgressBarValue(float value)
    {
        mProgressBar.Size += value;
    }

}
