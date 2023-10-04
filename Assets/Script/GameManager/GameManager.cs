using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using System.Linq;
using System.Xml;
using UnityEngine.Events;

/// <summary>
/// ��Ϸ������
/// </summary>
public class GameManager : MonoBehaviour, ISave
{
    public static GameManager instance;
    public int baseLuck;//��������ֵ
    public int additionLuck;//��������ֵ
    //public int 
    public enum Luckday
    {
        angel, devil, normal
    }
    public Luckday luckday;//������

    public List<ItemDetails> goodFishes = new List<ItemDetails>();//�õ���
    public List<ItemDetails> badFishes = new List<ItemDetails>();//�����
    public List<ItemDetails> allFishes = new List<ItemDetails>();//ȫ������

    // public GameObject fishPrefab;//���Ԥ����
    public GameObject fishingUI;//�����С��Ϸ

    public ItemDataList_SO itemDataList_SO;//��Ʒ����

    private float sleepTime;//˯��ʱ��

    public SlotUI fishingRod;//�����
    public SlotUI bait;//���

    public int PlayerSleepTime = 360;
    public int PlayerSleepCount;
    public bool Debuff = false;
    // public List<SlotUI> altarSlots= new List<SlotUI>();//�׼�����

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
        //���δ˯���¼�
        TimeManager.Instance.Day_Event.Add("ÿ��������һ��˯��ʱ��", () =>
        {
            PlayerSleepCount++;
            if (PlayerSleepCount >= 3)
            {
                PlayerSleepCount = 0;
                PlayerSleepTime -= 60;
            }
        });
        Debug.Log("����");
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
                var time = TimeManager.Instance.Game_Time.Copy();
                time.AddMinute(Random.Range(3, 6) * 60, false);
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
                        time1.AddMinute(Random.Range(3, 6) * 60, false);
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
    // Update is called once per frame
    void Update()
    {

    }

    Coroutine MiniGame = null;
    /// <summary>
    /// �������
    /// </summary>
    public void ClickToFishes()
    {
        if (MiniGame == null)
        {
            MiniGame = StartCoroutine(Fishing(1001));
        }
    }

    /// <summary>
    /// �����
    /// </summary>
    private void ClassifyFishes()
    {
        float _standardWeight = 2f;//���������������ע
        foreach (var _item in itemDataList_SO.itemDetailsList)
        {

            //�������������
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

            //������������
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
    /// ��������뱳��
    /// </summary>
    public ItemDetails AddFishes(MiniResultType _result)
    {
        Debug.Log("��ȡ��");
        int _weight = 0;//Ȩ��
        ItemType _type = new();//����
        switch (_result)
        {
            case MiniResultType.��ͨ�ɹ�:
                _type = ItemType.smallFish;
                break;
            case MiniResultType.��������:
                _type = ItemType.bigFish;
                break;
            default:
                Debug.Log("�������");
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
                    Debug.Log("������id:" + _fish.itemID + "��" + _fish.itemName);
                    _fish.itemWeight += Random.Range(-1, 2);//�������
                    return _fish;//������
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Ͷ����
    /// </summary>
    private int DropDice(int _a, int _min, int _max)
    {
        _a = Random.Range(_min, _max);
        return _a;
    }

    /// <summary>
    /// ��������ʹ���Ƕ�ħ
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
    /// ����ֵ����
    /// </summary>
    public void CountLucky()
    {
        if (bait.itemDetails != null)//����������
        {
            additionLuck += fishingRod.itemDetails.itemLucky + bait.itemDetails.itemLucky;
        }
        else if (bait.itemDetails == null)//������������
        {
            additionLuck = fishingRod.itemDetails.itemLucky;
        }

    }

    MiniResultType MiniResult = MiniResultType.�쳣;
    /// <summary>
    /// �����С��Ϸ
    /// </summary>
    IEnumerator Fishing(int Id)
    {
        yield return null;
        //��������ֵ�������ʱ��
        MiniGameManager.Instance.DescTimeMax = 100 - (baseLuck + additionLuck) + Random.Range(0, 10);

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

    public void Save()
    {

    }

    public void Load(string path)
    {
        throw new System.NotImplementedException();
    }

    // /// <summary>
    // /// ��ʼ�׼�
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
