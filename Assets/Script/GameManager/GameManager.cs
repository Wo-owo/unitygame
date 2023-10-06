using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

/// <summary>
/// 游戏控制器
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //public int 
    public enum Luckday
    {
        angel, devil, normal
    }
    public Luckday luckday;//幸运日



    // public GameObject fishPrefab;//鱼的预制体
    public GameObject fishingUI;//钓鱼得小游戏

    public ItemDataList_SO itemDataList_SO;//物品数据

    private float sleepTime;//睡觉时间

    public SlotUI fishingRod;//鱼竿栏
    public SlotUI bait;//鱼饵
    public InventoryBag_SO playerbag;//玩家背包
    public int PlayerSleepTime = 360;
    public int PlayerSleepCount;
    public bool Debuff = false;
    // public List<SlotUI> altarSlots= new List<SlotUI>();//献祭格子

    public int baseLuck;//基础幸运值
    public int additionLuck;//附加幸运值
    //public List<ItemDetails> lakeFishes= new List<ItemDetails>();//湖鱼
    //public List<ItemDetails> seaFishes = new List<ItemDetails>();//海鱼
    //public List<ItemDetails> allFishes = new List<ItemDetails>();//全部的鱼
    public List<ItemDetails> tempFishes = new List<ItemDetails>();
    public ItemDetails rubbish;

    
    public int fishingLevel;//钓鱼等级
    
    public TMP_Text locationText; // 用于显示当前所在地的文本
    public UnityEngine.UI.Button switchLocationButton; // 切换所在地的按钮,Debug用

    public UnityEngine.UI.Image lakeImg;//湖边背景图片
    public  UnityEngine.UI.Image seaImg;//海边背景图片
    // public  UnityEngine.UI.Image backGroundImg;//背景图片
    public bool isBySea = true; // 默认在海边
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);

    }
    // Start is called before the first frame update
    void Start()
    {
        isBySea=false;
        locationText.text = isBySea?"海边":"湖边";
        fishingLevel=1;
        itemDataList_SO = InventoryManager.Instance.itemDataList_SO;
        //ClassifyFishes();
        //添加未睡觉事件
        TimeManager.Instance.Day_Event.Add("每三天削减一次睡觉时间", () =>
        {
            //判定天气
            int a = UnityEngine.Random.Range(0, 100);
            if (a < 20)
            {
                WeatherManager.instance.StartRain(true);
                isRain=true;
            }
            else
            {
                WeatherManager.instance.StartRain(true);
                isRain=false;
            }
            PlayerSleepCount++;
            if (PlayerSleepCount >= 3)
            {
                PlayerSleepCount = 0;
                PlayerSleepTime -= 60;
            }

            LuckChange_Day();//判定今天是幸运日
        });
        //游戏开始时给拷贝一份
        WakeUpTime = TimeManager.Instance.Game_Time.Copy();
        WakeUpTime.AddMinute(1, false);
        //每小时判断当前时间与上一次睡觉时间是否有12小时
        TimeManager.Instance.Game_Time.HourChangedAll += () =>
        {
            if (WakeUpTime.GetToMinute() > 0 && TimeManager.Instance.Game_Time - WakeUpTime >= 720)
            {
                Debuff = true;
                WakeUpTime = new GameTimeDate();
                GameTimeDate time = TimeManager.Instance.Game_Time.Copy();
                time.AddMinute(UnityEngine.Random.Range(3, 6) * 60, false);
                var hour = time.Hour;
                TimeManager.Instance.TakeInHourEvent(hour, () =>
                {
                    if (Debuff == true)
                    {
                        Debug.Log($"疲劳,当前时间{TimeManager.Instance.Game_Time}");
                        TimeManager.Instance.PlayerSleep(60);
                        var e = TimeManager.Instance.Hour_Event[hour];
                        TimeManager.Instance.Hour_Event[hour] = null;
                        var time1 = TimeManager.Instance.Game_Time.Copy();
                        time1.AddMinute(UnityEngine.Random.Range(3, 6) * 60, false);
                        hour = time1.Hour;
                        Debug.Log($"下次疲劳,{time1}");
                        TimeManager.Instance.Hour_Event[hour] = e;
                    }
                    else
                    {
                        TimeManager.Instance.Hour_Event[hour] = null;
                    }
                });
            }
        };
    }
    public GameTimeDate WakeUpTime;
    //睡觉按钮触发事件
    public void PlayerSleep()
    {
        TimeManager.Instance.PlayerSleep(PlayerSleepTime);
        Debuff = false;
        WakeUpTime = TimeManager.Instance.Game_Time.Copy();
        WakeUpTime.AddMinute(PlayerSleepTime, false);
    }

    Coroutine MiniGame = null;
    /// <summary>
    /// 点击钓鱼
    /// </summary>
    public void ClickToFishes()
    {
        if (MiniGame == null)
        {
            MiniGame = StartCoroutine(Fishing(1));
        }
    }

    /// <summary>
    /// 切换地点
    /// </summary>
    public void SwitchLocation()
    {
        // 切换所在地
        isBySea = !isBySea;

        // 更新显示
        locationText.text = isBySea ? "海边" : "湖边";
        if(isBySea){
             seaImg.gameObject.SetActive(true);
        }
        else {
            seaImg.gameObject.SetActive(false);
        }

    }
    public bool isRain;
    /// <summary>
    /// 改变天气
    /// </summary>
    public void WeatherChange()
    {
        isRain=!isRain;
        WeatherManager.instance.StartRain(isRain);
    }

    /// <summary>
    /// 将鱼添加入背包
    /// </summary>
    public ItemDetails AddFishes(MiniResultType _result)
    {

        tempFishes.Clear();

        int _temp = 0;
        if (luckday == Luckday.angel)
        {
            _temp += 1;
        }
        else if (luckday == Luckday.devil)
        {
            _temp -= 1;
        }

        // 根据所在地选择鱼池
        //如果在海边
        if (isBySea)
        {
            tempFishes = itemDataList_SO.itemDetailsList
                .Where(fish => (fish.habitat == Habitat.sea || fish.habitat == Habitat.everywhere) && fish.rareDegree <= fishingLevel + _temp)
                .ToList();
            
        }
        else
        {
            tempFishes = itemDataList_SO.itemDetailsList
                .Where(fish => (fish.habitat == Habitat.lake || fish.habitat == Habitat.everywhere) && fish.rareDegree <= fishingLevel + _temp)
                .ToList();

        }
        string debugInfo = "";
        foreach(var _fish in tempFishes){
            debugInfo+=_fish.itemName+",";
        }
        Debug.Log("抽取鱼:"+debugInfo);

        int totalWeight = 0;//权重

        //Debug.Log(_type);
        foreach (ItemDetails fish in tempFishes)
        {
            totalWeight += fish.itemChance;
        }

        // 生成一个随机数
        int a = 0;
        switch (_result)
        {
            case MiniResultType.普通成功:
                //_type = ItemType.smallFish;
                a = UnityEngine.Random.Range(0, totalWeight);
                break;
            case MiniResultType.完美钓起:
                a = UnityEngine.Random.Range(totalWeight / 2, totalWeight);
                break;
            case MiniResultType.钓鱼失败:
                //直接返回一个垃圾()
                //return 
                break;
            default:
                Debug.Log("其他情况");
                break;
        }

        // 使用加权算法选择鱼
        int cumulativeWeight = 0;
        foreach (ItemDetails fish in tempFishes)
        {
            cumulativeWeight += fish.itemChance;
            if (a < cumulativeWeight)
            {
                fish.itemWeight = UnityEngine.Random.Range(fish.minWeight, fish.maxWeight);
                Debug.Log("钓上了"+fish.itemName);
                return fish;
            }
        }

        // 如果出现问题，返回 null 或者默认鱼类
        return null;


    }

    /// <summary>
    /// 投骰子
    /// </summary>
    private int DropDice(int _a, int _min, int _max)
    {
        _a = UnityEngine.Random.Range(_min, _max);
        return _a;
    }

    /// <summary>
    /// 今日是天使还是恶魔
    /// </summary>
    public void LuckChange_Day()
    {
        additionLuck = 0;
        int a = UnityEngine.Random.Range(0, 100);
        if (a < 20)
        {
            luckday = Luckday.angel;
            additionLuck += 5;
        }
        else if (a > 80)
        {
            luckday = Luckday.devil;
            additionLuck -= 5;
        }
        else
        {
            luckday = Luckday.normal;
        }
        //CountLucky();
    }


    /// <summary>
    /// 幸运值计算
    /// </summary>
    public void CountLucky()
    {
        additionLuck = 0;
        foreach (var _item in playerbag.itemList)
        {
            ItemDetails item = InventoryManager.Instance.GetItemDetails(_item.itemID);
            
            if (item.itemType == ItemType.Bait)
            {
                additionLuck += item.itemLuck;
            }
            if (item.itemType == ItemType.FishingRod)
            {
                additionLuck += item.itemLuck;
            }

        }
        if (luckday == Luckday.angel)
        {
            additionLuck += 20;
        }
        else if (luckday == Luckday.devil)
        {
            additionLuck -= 10;
        }
    }

    MiniResultType MiniResult = MiniResultType.异常;
    /// <summary>
    /// 钓鱼的小游戏
    /// </summary>
    IEnumerator Fishing(int Id)
    {
        yield return null;
        //根据幸运值计算钓鱼时间
        //  MiniGameManager.Instance.DescTimeMax = 100 - (baseLuck + additionLuck);

        MiniGameManager.Instance.StartGame(Id);
        enabled = false;
        while (MiniGameManager.Instance.IsStart)
        {
            yield return null;
        }
        MiniResult = MiniGameManager.Instance.MiniResult;
        if (MiniResult != MiniResultType.异常 && MiniResult != MiniResultType.钓鱼失败)
        {
            Debug.Log("钓鱼完成,钓鱼结果:" + MiniResult.ToString());
            var fish = AddFishes(MiniResult);
            if (fish != null)
            {
                InventoryManager.Instance.AddItem(fish.itemID, 1);
            }
            else
            {
                Debug.Log("什么都没有钓上");
            }
        }
        else
        {

        }
        enabled = true;
        MiniGame = null;
    }



}
