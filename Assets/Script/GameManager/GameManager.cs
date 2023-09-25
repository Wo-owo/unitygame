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

    //public List<Fishes> goodfish = new List<Fishes>();//好的鱼
    //public List<Fishes> badfish = new List<Fishes>();//差的鱼
    public List<Fishes> allFish = new List<Fishes>();
    public GameObject fishPrefab;//鱼的预制体


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
        // 从所有的鱼中随机选择一条
        int randomIndex = Random.Range(0, allFish.Count);
        Fishes fishToCatch = allFish[randomIndex];

        // 创建鱼的预制体
        GameObject fishObject = Instantiate(fishPrefab);
        FishPrefab _fishPrefab = fishObject.GetComponent<FishPrefab>();
        _fishPrefab.fishName = fishToCatch.fishName;

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

    
}
