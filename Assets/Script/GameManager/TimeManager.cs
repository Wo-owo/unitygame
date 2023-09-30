using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : Singleton<TimeManager>
{
    public GameTimeDate Game_Time = new GameTimeDate();
    public float TimeTimingUnit;
    float _timeTiming;
    public readonly Dictionary<int, UnityAction> Hour_Event = new();
    /// <summary>
    /// 添加指定时间[0,24)小时区间发生事件
    /// </summary>
    /// <param name="hour">指定时间</param>
    /// <param name="action">指定发生的方法委托</param>
    public void TakeInHourEvent(int hour, UnityAction action)
    {
        if(Hour_Event.ContainsKey(hour))
        {
            Hour_Event[hour] += action;
            return;
        }
        Hour_Event.Add(hour, action);
    }
    public void DeleteHourEvent(int hour, UnityAction action)
    {
        if (Hour_Event.ContainsKey(hour))
        {
            Hour_Event[hour] -= action;
            return;
        }
    }
    private void Start()
    {
        Game_Time.HourChanged += (h) =>
        {
            Hour_Event[h]?.Invoke();
        };//监听小时改变事件
        var action = new UnityAction(() => Debug.Log("该睡觉了"));
        TakeInHourEvent(1, action);//添加20点睡觉
        DeleteHourEvent(1, action);
    }
    // Update is called once per frame
    void Update()
    {
        _timeTiming += Time.deltaTime;
        if (_timeTiming >= TimeTimingUnit)
        {
            Game_Time.Minute++;
            _timeTiming = 0;
        }
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 200, 30), Game_Time.ToString());
    }
}
[Serializable]
public class GameTimeDate
{
    [SerializeField]
    private int minute;
    [SerializeField]
    private int hour;
    [SerializeField]
    private int day;
    [SerializeField]
    private int month;
    [SerializeField]
    private int year;

    public int Minute
    {
        get => minute; set
        {
            minute = value;
            if (minute == 60)
            {
                minute = 0;
                Hour++;
            }
        }
    }
    public int Hour
    {
        get => hour; set
        {
            hour = value;
            if (hour == 24)
            {
                hour = 0;
                Day++;
            }
            HourChanged?.Invoke(hour);
        }
    }
    public int Day
    {
        get => day; set
        {
            day = value;
            if (day == 30)
            {
                day = 1;
                Month++;
            }
        }
    }
    public int Month
    {
        get => month; set
        {
            month = value;
            if (month == 12)
            {
                month = 1;
                year++;
            }
        }
    }
    public int Year
    {
        get => year; set
        {
            year = value;
        }
    }
    public event UnityAction<int> HourChanged;
    public GameTimeDate()
    {
        day = 1;
        month = 1;
    }
    public override string ToString()
    {
        return $"{Year}年 {Month}月 {Day}日 {Hour}:{Minute}";
    }
}