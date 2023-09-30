using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public GameTimeDate Game_Time = new GameTimeDate();
    public int TimeTimingUnit;
    float _timeTiming;
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