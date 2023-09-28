using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏控制器
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int luck ;//幸运值
    //public int 

    //public List<Fishes> goodfish = new List<Fishes>();//好的鱼
    //public List<Fishes> badfish = new List<Fishes>();//差的鱼
    public List<Fishes> allFish = new List<Fishes>();
    public GameObject fishPrefab;//鱼的预制体
    public GameObject fishingUI;//钓鱼得小游戏

    public ItemDataList_SO itemDataList_SO;//物品数据



    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            if(instance != this)
            {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 点击钓鱼
    /// </summary>
    public void ClickToFishes(){
       
        var hasSpawned = false;
        foreach (var id_so in itemDataList_SO.itemDetailsList){
            if(luck>id_so.itemLucky){
                int a = 0;
                //投骰子判定
                a=DropDice(a,0,luck);
                //如果数值大于基础幸运值
                if(a>luck){
                    //生成这条鱼
                    hasSpawned = true;
                    ItemDetails randomItem = id_so;
                    InventoryManager.Instance.AddItem(randomItem.itemID,1);
                }
                break;
            }
        }
        //如果上述未生成鱼,则随机给一条低于幸运值的鱼
        if(!hasSpawned){
            // // int randomIndex = Random.Range(0, luck);//用幸运值做列表上限
            // ItemDetails randomItem = itemDataList_SO.itemDetailsList[randomIndex];
            // InventoryManager.Instance.AddItem(randomItem.itemID,1,false);
        }

         // 从所有的物品中随机选择一条的算法
        // if (itemDataList_SO != null && itemDataList_SO.itemDetailsList.Count > 0)
        // {
        //     //int randomIndex = Random.Range(0, luck);//用幸运值做列表上限
        //     int randomIndex = Random.Range(0, itemDataList_SO.itemDetailsList.Count);
        //     ItemDetails randomItem = itemDataList_SO.itemDetailsList[randomIndex];

            
        //     InventoryManager.Instance.AddItem(randomItem.itemID,1,false);
        // }

    }

    /// <summary>
    /// 投骰子
    /// </summary>
    private int DropDice(int _a,int _min,int _max){
        _a = Random.Range(_min,_max);
        return _a;
    }

    /// <summary>
    /// 时间流逝
    /// </summary>
    private void TimeTick(){

    }

    /// <summary>
    /// 幸运值计算
    /// </summary>
    void CountLucky(){
         
    }

    /// <summary>
    /// 钓鱼的小游戏
    /// </summary>
    void Fishing(){

    }
}
