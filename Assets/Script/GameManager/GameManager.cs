using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ������
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int luck;//����ֵ
    //public int 

    //public List<Fishes> goodfish = new List<Fishes>();//�õ���
    //public List<Fishes> badfish = new List<Fishes>();//�����
    public List<ItemDetails> allFish = new List<ItemDetails>();
    public GameObject fishPrefab;//���Ԥ����
    public GameObject fishingUI;//�����С��Ϸ

    public ItemDataList_SO itemDataList_SO;//��Ʒ����

    private float sleepTime;//˯��ʱ��

    public SlotUI fishingRod;//�����
    public SlotUI bait;//���

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
    /// ��������뱳��
    /// </summary>
    public ItemDetails AddFishes(MiniResultType _result)
    {
        Debug.Log("��ȡ��");
        int totalLuck = 0;//Ȩ��
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
                    Debug.Log("������id:" + _fish.itemID + "��" + _fish.itemName);
                    _fish.itemWeight += Random.Range(-10, 11);//�������


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
    /// ����ֵ����
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

    MiniResultType MiniResult = MiniResultType.�쳣;
    /// <summary>
    /// �����С��Ϸ
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
        if (MiniResult != MiniResultType.�쳣)
        {
            Debug.Log("�������,������:" + MiniResult.ToString());
            var fish = AddFishes(MiniResult);
            if(fish != null)
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
