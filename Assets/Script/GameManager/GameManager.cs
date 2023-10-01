using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏控制器
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int luck;//幸运值
    //public int 

    //public List<Fishes> goodfish = new List<Fishes>();//好的鱼
    //public List<Fishes> badfish = new List<Fishes>();//差的鱼
    public List<ItemDetails> allFish = new List<ItemDetails>();
    public GameObject fishPrefab;//鱼的预制体
    public GameObject fishingUI;//钓鱼得小游戏

    public ItemDataList_SO itemDataList_SO;//物品数据

    private float sleepTime;//睡觉时间

    public SlotUI fishingRod;//鱼竿栏
    public SlotUI bait;//鱼饵

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
    /// 将鱼添加入背包
    /// </summary>
    public ItemDetails AddFishes(MiniResultType _result)
    {
        Debug.Log("抽取鱼");
        int totalLuck = 0;//权重
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


        foreach (var _fish in allFish)
        {
            if (_fish.itemLucky < luck && _fish.itemType == _type)
            {
                totalLuck += _fish.itemLucky;
            }
        }
        int randomValue = Random.Range(0, totalLuck);
        foreach (var _fish in allFish)
        {
            if (_fish.itemLucky < luck && _fish.itemType == _type)
            {
                randomValue -= _fish.itemLucky;
                if (randomValue <= 0)
                {
                    Debug.Log("钓上了id:" + _fish.itemID + "的" + _fish.itemName);
                    _fish.itemWeight += Random.Range(-10, 11);//随机重量


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
    /// 幸运值计算
    /// </summary>
    public void CountLucky()
    {
        if (bait.itemDetails != null)
        {
            luck += fishingRod.itemDetails.itemLucky + bait.itemDetails.itemLucky;
        }
        else if (bait.itemDetails == null)
        {
            luck = fishingRod.itemDetails.itemLucky;
        }
    }

    MiniResultType MiniResult = MiniResultType.异常;
    /// <summary>
    /// 钓鱼的小游戏
    /// </summary>
    IEnumerator Fishing(int Id)
    {
        yield return null;
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
            if(fish != null)
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


}
