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
    private void Update()
    {
        time += Time.deltaTime;
        if (time > DescTimeMax && mProgressBar.Size < 1)
        {
            Fish.enabled = false;
            CatchFish.enabled = false;
            this.enabled = false;
            Debug.Log("Ê§°Ü");
            return;
        }
        if (mProgressBar.Size >= 1)
        {
            Fish.enabled = false;
            CatchFish.enabled = false;
            this.enabled = false;
            Debug.Log("³É¹¦");
            return;
        }
        if (Input.GetKey(Catch_MoveUP_Key))
        {
            CatchFish.transform.Translate(Vector2.up * Time.deltaTime * Catch_Move_Speed);
        }
        else
        {
            CatchFish.transform.Translate(-Vector2.up * Time.deltaTime * Catch_Move_Speed);
        }
    }
    public void SetProgressBarValue(float value)
    {
        mProgressBar.Size += value;
    }

}
