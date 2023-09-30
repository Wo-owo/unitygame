using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public GameTimeDate Game_Time = new GameTimeDate(1);
    // Update is called once per frame
    void Update()
    {
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 200, 30), Game_Time.ToString());
    }
}
public class GameTimeDate
{
    private int minute;
    private int hour;
    private int day;
    private int month;
    private int year;
    public int TimeTimingUnit;
    private Task _task;

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
    public void StopTiming()
    {
        _task.Wait();
    }
    /// <summary>
    /// </summary>
    /// <param name="timeTimingUnit">计时单位间隔</param>
    public GameTimeDate(int timeTimingUnit)
    {
        day = 1;
        month = 1;
        TimeTimingUnit = timeTimingUnit;
        _task = new Task(Timing);
        _task.Start();
    }
    public async void Timing()
    {
        while (true)
        {
            await Task.Delay(TimeTimingUnit);
            Minute += 1;
        }
    }
    public void TransformTime()
    {
        Minute += 1;
    }
    public override string ToString()
    {
        return $"{Year}年 {Month}月 {Day}日 {Hour}:{Minute}";
    }
    ~GameTimeDate()
    {
        _task.Dispose();
    }
}