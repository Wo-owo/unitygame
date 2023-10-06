using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// ��Ϸ������
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //public int 
    public enum Luckday
    {
        angel, devil, normal
    }
    public Luckday luckday;//������

 

    // public GameObject fishPrefab;//���Ԥ����
    public GameObject fishingUI;//�����С��Ϸ

    public ItemDataList_SO itemDataList_SO;//��Ʒ����

    private float sleepTime;//˯��ʱ��

    public SlotUI fishingRod;//�����
    public SlotUI bait;//���
    public InventoryBag_SO playerbag;//��ұ���
    public int PlayerSleepTime = 360;
    public int PlayerSleepCount;
    public bool Debuff = false;
    // public List<SlotUI> altarSlots= new List<SlotUI>();//�׼�����

    public int baseLuck;//��������ֵ
    public int additionLuck;//��������ֵ
    //public List<ItemDetails> lakeFishes= new List<ItemDetails>();//����
    //public List<ItemDetails> seaFishes = new List<ItemDetails>();//����
    //public List<ItemDetails> allFishes = new List<ItemDetails>();//ȫ������
    public List<ItemDetails> tempFishes = new List<ItemDetails>();
    public ItemDetails rubbish;
    
    public bool isBySea = true; // Ĭ���ں���
    public int fishingLevel;//����ȼ�

    public Text locationText; // ������ʾ��ǰ���ڵص��ı�
    public Button switchLocationButton; // �л����ڵصİ�ť,Debug��
    
    
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
        //ClassifyFishes();
        //���δ˯���¼�
        TimeManager.Instance.Day_Event.Add("ÿ��������һ��˯��ʱ��", () =>
        {
            int a = UnityEngine.Random.Range(0,100);
            if(a<20){
                WeatherManager.instance.StartRain(true);    
            }
            else {WeatherManager.instance.StartRain(true);    
            }
            PlayerSleepCount++;
            if (PlayerSleepCount >= 3)
            {
                PlayerSleepCount = 0;
                PlayerSleepTime -= 60;
            }
        });
        //��Ϸ��ʼʱ������һ��
        WakeUpTime = TimeManager.Instance.Game_Time.Copy();
        WakeUpTime.AddMinute(1, false);
        //ÿСʱ�жϵ�ǰʱ������һ��˯��ʱ���Ƿ���12Сʱ
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
                        Debug.Log($"ƣ��,��ǰʱ��{TimeManager.Instance.Game_Time}");
                        TimeManager.Instance.PlayerSleep(60);
                        var e = TimeManager.Instance.Hour_Event[hour];
                        TimeManager.Instance.Hour_Event[hour] = null;
                        var time1 = TimeManager.Instance.Game_Time.Copy();
                        time1.AddMinute(UnityEngine.Random.Range(3, 6) * 60, false);
                        hour = time1.Hour;
                        Debug.Log($"�´�ƣ��,{time1}");
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
    //˯����ť�����¼�
    public void PlayerSleep()
    {
        TimeManager.Instance.PlayerSleep(PlayerSleepTime);
        Debuff = false;
        WakeUpTime = TimeManager.Instance.Game_Time.Copy();
        WakeUpTime.AddMinute(PlayerSleepTime, false);
    }

    Coroutine MiniGame = null;
    /// <summary>
    /// �������
    /// </summary>
    public void ClickToFishes()
    {
        if (MiniGame == null)
        {
            MiniGame = StartCoroutine(Fishing(1));
        }
    }

    void SwitchLocation()
    {
        // �л����ڵ�
        isBySea = !isBySea;

        // ������ʾ
        locationText.text = isBySea ? "����" : "����";
    }
    /// <summary>
    /// �����
    /// </summary>
    private void ClassifyFishes()
    {
        // // �������ڵ�ѡ�����
        // //����ں���
        // if (isBySea)
        // {
        //     tempFishes = itemDataList_SO.itemDetailsList
        //         .Where(fish => (fish.habitat == ItemDetails.Habitat.sea || fish.habitat == ItemDetails.Habitat.everywhere) && fish.rareDegree <= fishingLevel)
        //         .ToList();
        // }
        // else
        // {
        //     tempFishes = itemDataList_SO.itemDetailsList
        //         .Where(fish => (fish.habitat == ItemDetails.Habitat.lake || fish.habitat == ItemDetails.Habitat.everywhere) && fish.rareDegree <= fishingLevel)
        //         .ToList();
        // }

        // // ʹ�ü�Ȩ�㷨ѡ����ϵ���
        // ItemDetails caughtFish = WeightedRandomFish(fishPool);

        // if (caughtFish != null)
        // {
        //     // ��������������
        //     float randomWeight = Random.Range(caughtFish.minWeight, caughtFish.maxWeight);
        //     caughtFish.itemWeight = randomWeight;

        //     // �������
        //     Debug.Log("������: " + caughtFish.name + ", ����: " + caughtFish.itemWeight + ",ϡ�ж�: " + caughtFish.rareDegree);
        // }
        // else
        // {
        //     // û��ƥ�����
        //     Debug.Log("û��ƥ�����");
        // }
    }
    
    /// <summary>
    /// ��������뱳��
    /// </summary>
    public ItemDetails AddFishes(MiniResultType _result)
    {

        tempFishes.Clear();

        int _temp = 0;
        if(luckday==Luckday.angel){
            _temp+=1;
        }
        else if(luckday==Luckday.devil){
            _temp-=1;
        }


        // �������ڵ�ѡ�����
        //����ں���
        if (isBySea)
        {
            tempFishes = itemDataList_SO.itemDetailsList
                .Where(fish => (fish.habitat == Habitat.sea || fish.habitat == Habitat.everywhere) && fish.rareDegree <= fishingLevel+_temp)
                .ToList();
            
        }
        else
        {
            tempFishes = itemDataList_SO.itemDetailsList
                .Where(fish => (fish.habitat == Habitat.lake || fish.habitat ==Habitat.everywhere) && fish.rareDegree <= fishingLevel+_temp)
                .ToList();
            
        }
        Debug.Log("��ȡ��");
        
        int totalWeight = 0;//Ȩ��
        
        //Debug.Log(_type);
        foreach (ItemDetails fish in tempFishes)
        {
            totalWeight += fish.itemChance;
        }

        // ����һ�������
        int a=0;
        switch (_result)
        {
            case MiniResultType.��ͨ�ɹ�:
                //_type = ItemType.smallFish;
                a = UnityEngine.Random.Range(0, totalWeight);
                break;
            case MiniResultType.��������:
                a = UnityEngine.Random.Range(totalWeight/2, totalWeight);
                break;
            case MiniResultType.����ʧ��:
                //ֱ�ӷ���һ������()
                //return 
                break;
            default:
                Debug.Log("�������");
                break;
        }

        // ʹ�ü�Ȩ�㷨ѡ����
        int cumulativeWeight = 0;
        foreach (ItemDetails fish in tempFishes)
        {
            cumulativeWeight += fish.itemChance;
            if (a < cumulativeWeight)
            {
                fish.itemWeight = UnityEngine.Random.Range(fish.minWeight,fish.maxWeight);
                return fish;
            }
        }

        // ����������⣬���� null ����Ĭ������
        return null;

        
    }

    /// <summary>
    /// Ͷ����
    /// </summary>
    private int DropDice(int _a, int _min, int _max)
    {
        _a = UnityEngine.Random.Range(_min, _max);
        return _a;
    }

    /// <summary>
    /// ��������ʹ���Ƕ�ħ
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
    /// ����ֵ����
    /// </summary>
    public void CountLucky()
    {
        additionLuck = 0;
        foreach(var _item in playerbag.itemList){
            ItemDetails item = InventoryManager.Instance.GetItemDetails(_item.itemID);
            if(item.itemType==ItemType.Bait){
                additionLuck+=item.itemLuck;
            }
            if(item.itemType==ItemType.FishingRod){
                additionLuck+=item.itemLuck;
            }
            
        }
        if(luckday==Luckday.angel){
            additionLuck+=20;
        }
        else if(luckday==Luckday.devil){
            additionLuck-=10;
        }


        
        // if (bait.itemDetails != null)//����������
        // {
        //     additionLuck += fishingRod.itemDetails.itemChance + bait.itemDetails.itemChance;
        // }
        // else if (bait.itemDetails == null)//������������
        // {
        //     additionLuck = fishingRod.itemDetails.itemChance;
        // }

    }

    MiniResultType MiniResult = MiniResultType.�쳣;
    /// <summary>
    /// �����С��Ϸ
    /// </summary>
    IEnumerator Fishing(int Id)
    {
        yield return null;
        //��������ֵ�������ʱ��
        MiniGameManager.Instance.DescTimeMax = 100 - (baseLuck + additionLuck) ;

        MiniGameManager.Instance.StartGame(Id);
        enabled = false;
        while (MiniGameManager.Instance.IsStart)
        {
            yield return null;
        }
        MiniResult = MiniGameManager.Instance.MiniResult;
        if (MiniResult != MiniResultType.�쳣)
        {
            Debug.Log("�������,������:" + MiniResult.ToString());
            var fish = AddFishes(MiniResult);
            if (fish != null)
            {
                InventoryManager.Instance.AddItem(fish.itemID, 1);
            }
            else
            {
                Debug.Log("ʲô��û�е���");
            }
        }
        enabled = true;
        MiniGame = null;
    }



}
