using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using static UnityEditor.Progress;

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
    //TODO��ʱͨ��button��ť����Ĭ��1001��Ʒ
    public void StartGame(int ID)
    {
        IsStart = true;
        fishID = ID;
        time = 0;
        mProgressBar.Size = 0;
        CatchFish.LeaveCount = 0;
        Fish.transform.Translate(new Vector2(0, Random.Range(-100, 100)));
        CatchFish.transform.Translate(new Vector2(0, Random.Range(-100, 100)));
    }
    private void Update()
    {
        if (!IsStart)
            return;
        time += Time.deltaTime;
        if (time > DescTimeMax && mProgressBar.Size < 1)
        {
            Debug.Log("ʧ��");
            IsStart = false;
            return;
        }
        if (mProgressBar.Size >= 1)
        {
            IsStart = false;
            if (CatchFish.LeaveCount > 0)
            {
                Debug.Log("�ɹ�");
                InventoryManager.Instance.AddItem(fishID, 1);//TODO��ʱֱ�ӵ�������������
            }
            else
            {
                Debug.Log("����");
                InventoryManager.Instance.AddItem(fishID, 1008611);//TODO��ʱֱ�ӵ�������������
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
