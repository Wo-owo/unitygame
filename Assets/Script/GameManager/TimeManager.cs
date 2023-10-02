﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        if (Hour_Event.ContainsKey(hour))
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
    GameTimeDate Timer;
    public void StartTimer(int m)
    {
        Debug.Log($"计时开始:{m}游戏分钟,当前游戏时间{Game_Time}");
        Timer = Game_Time.newGameTimeDate(m);
    }
    public bool GetTimerEnd() => Timer != null && Timer <= Game_Time;
    protected override void Awake()
    {
        base.Awake();
        Game_Time.HourChanged += (h) =>
        {
            if (Hour_Event.ContainsKey(h))
                Hour_Event[h]?.Invoke();
        };//监听小时改变事件
        var action = new UnityAction(() => Debug.Log("该睡觉了"));
        TakeInHourEvent(20, action);//添加20点睡觉
        //DeleteHourEvent(1, action);//移除事件
        StartTimer(10);
    }
    public bool Stop = false;
    // Update is called once per frame
    void Update()
    {
        if (Stop)
        {
            return;
        }
        _timeTiming += Time.deltaTime;
        if (_timeTiming >= TimeTimingUnit)
        {
            Game_Time.Minute++;
            _timeTiming = 0;
        }
        if (GetTimerEnd())
        {
            Debug.Log($"计时结束,当前游戏时间{Game_Time}");
            Timer = null;
        }
        if (canvas != null && Stop == false&& isSleep==true)
        {
            foreach (Canvas c in canvas)
            {
                c.gameObject.SetActive(true);
            }
            isSleep = false;
        }
    }
    bool isSleep;
    Canvas[] canvas;
    /// <summary>
    /// 执行休眠，直接跳过指定分钟
    /// </summary>
    /// <param name="m"></param>
    public void PlayerSleep(int m)
    {
        isSleep = true;
        Stop = true;
        if (canvas == null)
            canvas = Resources.FindObjectsOfTypeAll<Canvas>();
        foreach (Canvas c in canvas)
        {
            c.gameObject.SetActive(false);
        }
        Task.Run(async () =>
        {
            await Task.Delay(3000); 
            Stop = false;
            Game_Time.AddMinute(m,false);
        });
    }
    /// <summary>
    /// 执行休眠，直接跳过指定分钟并再跳过后执行指定委托
    /// </summary>
    /// <param name="m"></param>
    /// <param name="action"></param>
    public void PlayerSleep(int m, UnityAction action)
    {
        Game_Time.AddMinute(m, false);
        action?.Invoke();
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 200, 30), Game_Time.ToString());
        if (isSleep)
        {
            GUI.Box(new Rect(0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height), "一段时间过去了.....");
        }
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
    }//60
    public int Day
    {
        get => day; set
        {
            day = value;
            if (day == 30)
            {
                day = 0;
                Month++;
            }
        }
    }//24
    public int Month
    {
        get => month; set
        {
            month = value;
            if (month == 12)
            {
                month = 0;
                year++;
            }
        }
    }//30
    public int Year
    {
        get => year; set
        {
            year = value;
        }
    }//12
    public event UnityAction<int> HourChanged;
    public GameTimeDate()
    {
    }
    public GameTimeDate(int m)
    {
        for (int i = 0; i < m; i++)
        {
            Minute++;
        }
    }
    public void AddMinute(int m,bool trigeerEvent)
    {
        if (trigeerEvent)
        {
            for (int i = 0; i < m; i++)
            {
                Minute++;
            }
        }
        else
        {
            var e = HourChanged;
            HourChanged = null;
            for (int i = 0; i < m; i++)
            {
                Minute++;
            }
            HourChanged = e;
        }

    }
    public GameTimeDate newGameTimeDate(int m)
    {
        var time = new GameTimeDate();
        time.minute = this.minute;
        time.hour = this.hour;
        time.day = this.day;
        time.month = this.month;
        time.year = this.year;
        time.AddMinute(m, false);
        return time;
    }
    public override string ToString() => $"{Year}年 {Month + 1}月 {Day + 1}日 {(Hour >= 10 ? Hour : $"0{Hour}")}:{(Minute >= 10 ? Minute : $"0{Minute}")}";

    public int GetToMinute() => minute + hour * 60 + day * 60 * 24 + month * 60 * 24 * 30 + year * 60 * 24 * 30 * 12;
    public static bool operator >=(GameTimeDate a, GameTimeDate b)
    {
        return a.year >= b.Year && a.month >= b.month && a.day >= b.day && a.hour >= b.hour && a.minute >= b.minute;
    }
    public static bool operator <=(GameTimeDate a, GameTimeDate b)
    {
        return a.year <= b.Year && a.month <= b.month && a.day <= b.day && a.hour <= b.hour && a.minute <= b.minute;
    }
}