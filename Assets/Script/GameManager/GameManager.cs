using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using System.Linq;
using System.Xml;
using UnityEngine.Events;

/// <summary>
/// 游戏控制器
/// </summary>
public class GameManager : MonoBehaviour, ISave
{
    public static GameManager instance;
    public int baseLuck;//基础幸运值
    public int additionLuck;//附加幸运值
    //public int 
    public enum Luckday
    {
        angel, devil, normal
    }
    public Luckday luckday;//幸运日

    public List<ItemDetails> goodFishes = new List<ItemDetails>();//好的鱼
    public List<ItemDetails> badFishes = new List<ItemDetails>();//差的鱼
    public List<ItemDetails> allFishes = new List<ItemDetails>();//全部的鱼

    // public GameObject fishPrefab;//鱼的预制体
    public GameObject fishingUI;//钓鱼得小游戏

    public ItemDataList_SO itemDataList_SO;//物品数据

    private float sleepTime;//睡觉时间

    public SlotUI fishingRod;//鱼竿栏
    public SlotUI bait;//鱼饵

    public int PlayerSleepTime = 360;
    public int PlayerSleepCount;
    public bool Debuff = false;
    // public List<SlotUI> altarSlots= new List<SlotUI>();//献祭格子

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
        itemDataList_SO = InventoryManager.Instance.itemDataList_SO;
        ClassifyFishes();
        //添加未睡觉事件
        TimeManager.Instance.Day_Event.Add("每三天削减一次睡觉时间", () =>
        {
            PlayerSleepCount++;
            if (PlayerSleepCount >= 3)
            {
                PlayerSleepCount = 0;
                PlayerSleepTime -= 60;
            }
        });
        Debug.Log("测试");
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
                var time = TimeManager.Instance.Game_Time.Copy();
                time.AddMinute(Random.Range(3, 6) * 60, false);
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
                        time1.AddMinute(Random.Range(3, 6) * 60, false);
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
    // Update is called once per frame
    void Update()
    {

    }

    Coroutine MiniGame = null;
    /// <summary>
    /// 点击钓鱼
    /// </summary>
    public void ClickToFishes()
    {
        if (MiniGame == null)
        {
            MiniGame = StartCoroutine(Fishing(1001));
        }
    }

    /// <summary>
    /// 鱼分类
    /// </summary>
    private void ClassifyFishes()
    {
        float _standardWeight = 2f;//衡量好鱼的重量标注
        foreach (var _item in itemDataList_SO.itemDetailsList)
        {

            //根据鱼种类分类
            // if(_item.itemType == ItemType.smallFish){
            //     badFishes.Add(_item);
            // }
            // else if(_item.itemType == ItemType.bigFish){
            //     if(_item.itemWeight>_standardWeight){
            //         goodFishes.Add(_item);
            //     }
            //     else{
            //         badFishes.Add(_item);
            //     }
            // }
            // else if(_item.itemType == ItemType.rareFish){
            //     goodFishes.Add(_item);
            // }

            //根据重量划分
            if (_item.itemType == ItemType.Fish || _item.itemType == ItemType.smallFish || _item.itemType == ItemType.bigFish)
            {
                if (_item.itemWeight >= _standardWeight)
                {
                    goodFishes.Add(_item);
                }
                else
                {
                    badFishes.Add(_item);
                }
                allFishes.Add(_item);
            }
        }
    }
    /// <summary>
    /// 将鱼添加入背包
    /// </summary>
    public ItemDetails AddFishes(MiniResultType _result)
    {
        Debug.Log("抽取鱼");
        int _weight = 0;//权重
        ItemType _type = new();//类型
        switch (_result)
        {
            case MiniResultType.普通成功:
                _type = ItemType.smallFish;
                break;
            case MiniResultType.完美钓起:
                _type = ItemType.bigFish;
                break;
            default:
                Debug.Log("其他情况");
                break;
        }
        Debug.Log(_type);
        List<ItemDetails> _temp = new();
        if (luckday == Luckday.angel)
        {
            _temp = goodFishes;
        }
        else if (luckday == Luckday.devil)
        {
            _temp = badFishes;
        }
        else if (luckday == Luckday.normal)
        {
            _temp = allFishes;
        }

        foreach (var _fish in _temp)
        {
            if (_fish.itemLucky < baseLuck + additionLuck && _fish.itemType == _type)
            {
                _weight += _fish.itemLucky;
            }
        }
        int randomValue = Random.Range(0, _weight);
        foreach (var _fish in _temp)
        {
            if (_fish.itemLucky < baseLuck + additionLuck && _fish.itemType == _type)
            {
                randomValue -= _fish.itemLucky;
                if (randomValue <= 0)
                {
                    Debug.Log("钓上了id:" + _fish.itemID + "的" + _fish.itemName);
                    _fish.itemWeight += Random.Range(-1, 2);//随机重量
                    return _fish;//返回鱼
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 投骰子
    /// </summary>
    private int DropDice(int _a, int _min, int _max)
    {
        _a = Random.Range(_min, _max);
        return _a;
    }

    /// <summary>
    /// 今日是天使还是恶魔
    /// </summary>
    public void LuckChange_Day()
    {
        additionLuck = 0;
        int a = Random.Range(0, 100);
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
        CountLucky();
    }


    /// <summary>
    /// 幸运值计算
    /// </summary>
    public void CountLucky()
    {
        if (bait.itemDetails != null)//如果存在鱼饵
        {
            additionLuck += fishingRod.itemDetails.itemLucky + bait.itemDetails.itemLucky;
        }
        else if (bait.itemDetails == null)//如果不存在鱼饵
        {
            additionLuck = fishingRod.itemDetails.itemLucky;
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
        MiniGameManager.Instance.DescTimeMax = 100 - (baseLuck + additionLuck) + Random.Range(0, 10);

        MiniGameManager.Instance.StartGame(Id);
        enabled = false;
        while (MiniGameManager.Instance.IsStart)
        {
            yield return null;
        }
        MiniResult = MiniGameManager.Instance.MiniResult;
        if (MiniResult != MiniResultType.异常)
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
        enabled = true;
        MiniGame = null;
    }

    public void Save()
    {

    }

    public void Load(string path)
    {
        throw new System.NotImplementedException();
    }

    // /// <summary>
    // /// 开始献祭
    // /// </summary>
    // public void StartAltar(){
    //     int _temp = 0;
    //     foreach(var _slot in altarSlots){
    //         if(_slot.itemDetails.itemType == ItemType.Fish){

    //             if(_slot.itemDetails.itemType == ItemType.smallFish){
    //                 _temp +=2;
    //             }
    //         }
    //         else if(_slot.itemDetails==null){

    //         }
    //     }
    //     baseLuck+=_temp;
    // }
}
