using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFarm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrefab;
        public Item bounceItemPrefab;
        private Transform itemParent;

        private Transform PlayerTransform => FindObjectOfType<Player>().transform;

        //��¼����Item
        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();

        private void OnEnable()
        {
            EventHeadler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHeadler.DropItemEvent += OnDropItemEvent;
            EventHeadler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHeadler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        }

        private void OnDisable()
        {
            EventHeadler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHeadler.DropItemEvent -= OnDropItemEvent;
            EventHeadler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHeadler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        }

        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItems();
        }

        private void OnAfterSceneLoadedEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            RecreateAllItems();
        }

       

        private void OnInstantiateItemInScene(int ID,Vector3 pos)
        {
            var item=Instantiate(itemPrefab,pos,Quaternion.identity);
            item.itemID = ID;
        }
        /// <summary>
        /// ��ȡ��ǰ��������item
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> currentSceneItems =new List<SceneItem>();

            foreach (var item in FindObjectsOfType<Item>())
            {
                SceneItem sceneItem = new SceneItem
                {
                    itemID = item.itemID,
                    position = new SerializableVector3(item.transform.position)
                };
                currentSceneItems.Add(sceneItem);
            }

            if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                //�ҵ����ݾ͸���item�����б�
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItems;
            }
            else //������³���
            {
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItems);
            }
        }
        /// <summary>
        /// ˢ���ؽ������е�����
        /// </summary>
        private void RecreateAllItems()
        {
            List<SceneItem> currentSceneItems=new List<SceneItem>(); 

            if(sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name,out currentSceneItems))
            {
                if (currentSceneItems != null)
                {
                    //�峡
                    foreach (var item in FindObjectsOfType<Item>())
                    {
                        Destroy(item.gameObject);
                    }
                    foreach (var item in currentSceneItems)
                    {
                        Item newItem = Instantiate(itemPrefab, item.position.ToVector3(), Quaternion.identity, itemParent);
                        newItem.Init(item.itemID);
                    }
                }
            }
        }
        private void OnDropItemEvent(int ID, Vector3 mousePos, ItemType type)
        {
            Item item = Instantiate(bounceItemPrefab, PlayerTransform.position, Quaternion.identity);
            item.itemID = ID;
            Vector3 dir = (mousePos - PlayerTransform.position).normalized;
            item.GetComponent<ItemBounce>().InitBounceItem(mousePos,dir);
        }
    }
}