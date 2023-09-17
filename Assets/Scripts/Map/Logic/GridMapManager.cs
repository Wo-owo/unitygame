using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using MFarm.CropPlant;

namespace MFarm.Map
{
    public class GridMapManager : Singleton<GridMapManager>
    {       
        [Header("种地瓦片切换信息")]
        public RuleTile digTile;
        public RuleTile waterTile;

        private Tilemap digTileMap;
        private Tilemap waterTileMap;

        [Header("地图信息")]
        public List<MapData_SO> mapDataList;

        private Season currentSeason;

        //场景名字+坐标和对应的瓦片信息
        private Dictionary<string ,TileDetails> tileDetailsDict = new Dictionary<string ,TileDetails>();

        private Dictionary<string,bool> firstLoadDict = new Dictionary<string ,bool>();

        private Grid currentGrid;

        //杂草列表
        private List<ReapItem> itemsInRadius;

        private void OnEnable()
        {
            EventHeadler.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;
            EventHeadler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
            EventHeadler.GameDayEvent += OnGameDayEvent;
            EventHeadler.RefreshCurrentMap += RefreshMap;
        }

        private void OnDisable()
        {
            EventHeadler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
            EventHeadler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
            EventHeadler.GameDayEvent -= OnGameDayEvent;
            EventHeadler.RefreshCurrentMap -= RefreshMap;
        }

        private void Start()
        {
            foreach(MapData_SO mapData in mapDataList)
            {
                firstLoadDict.Add(mapData.sceneName, true);
                InitTileDetailsDict(mapData);
            }
        }
        /// <summary>
        /// 根据地图信息生成字典
        /// </summary>
        /// <param name="mapData">地图信息</param>
        private void InitTileDetailsDict(MapData_SO mapData)
        {
            foreach (TileProperty tileProperty in mapData.tileProperties)
            {
                TileDetails tileDetails = new TileDetails
                {
                    gridX = tileProperty.tileCoordinate.x,
                    gridY = tileProperty.tileCoordinate.y
                };

                //字典的Key
                string key = tileDetails.gridX + "x" + tileDetails.gridY + "y" + mapData.sceneName;

                if (GetTileDetails(key) != null)
                {
                    tileDetails = GetTileDetails(key);
                }

                switch (tileProperty.gridType)
                {
                    case GridType.Diggable:
                        tileDetails.canDig = tileProperty.boolTypeValue;
                        break;
                    case GridType.DropItem:
                        tileDetails.canDropItem = tileProperty.boolTypeValue;
                        break;
                    case GridType.PlaceFurniture:
                        tileDetails.canPlaceFurniture = tileProperty.boolTypeValue;
                        break;
                    case GridType.NPCObstacle:
                        tileDetails.isNPCObstacle = tileProperty.boolTypeValue;
                        break;
                }

                if (GetTileDetails(key) != null)
                    tileDetailsDict[key] = tileDetails;
                else
                    tileDetailsDict.Add(key, tileDetails);
            }
        }
        /// <summary>
        /// 根据key返回瓦片信息
        /// </summary>
        /// <param name="key">x+y+name</param>
        /// <returns></returns>
        private TileDetails GetTileDetails(string key)
        {
            if (tileDetailsDict.ContainsKey(key))
            {
                return tileDetailsDict[key];
            }
            return null;
        }
        /// <summary>
        /// 根据网格坐标返回瓦片信息
        /// </summary>
        /// <param name="mouseGridPos"></param>
        /// <returns></returns>
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos)
        {
            string key = mouseGridPos.x + "x" + mouseGridPos.y + "y" + SceneManager.GetActiveScene().name;
            return GetTileDetails(key);
        }
        private void OnAfterSceneLoadedEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            digTileMap=GameObject.FindWithTag("Dig").GetComponent<Tilemap>();
            waterTileMap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();

            if (firstLoadDict[SceneManager.GetActiveScene().name])
            {
                //DisplayMap(SceneManager.GetActiveScene().name);
                EventHeadler.CallGenerateCropEvent();
                firstLoadDict[SceneManager.GetActiveScene().name] = false;
            }
            RefreshMap();
        }
        /// <summary>
        /// 执行实际工具或物品功能
        /// </summary>
        /// <param name="mouseWorldPos">鼠标坐标</param>
        /// <param name="itemDetails">物品信息</param>
        private void OnExecuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            Vector3Int mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
            TileDetails currentTile = GetTileDetailsOnMousePosition(mouseGridPos);

            if (currentTile != null)
            {
                //    Crop currentCrop = GetCropObject(mouseWorldPos);

                //    //WORKFLOW:物品使用实际功能
                switch (itemDetails.itemType)
                {
                    case ItemType.Seed:
                        EventHeadler.CallPlantSeedEvent(itemDetails.itemID, currentTile);
                        EventHeadler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos, itemDetails.itemType);
                        //EventHeadler.CallPlaySoundEvent(SoundName.Plant);
                        break;
                    case ItemType.Commodity:
                        EventHeadler.CallDropItemEvent(itemDetails.itemID, mouseGridPos,ItemType.Commodity);
                        break;
                    case ItemType.HoeTool:
                        SetDigGround(currentTile);
                        currentTile.daysSinceDug = 0;
                        currentTile.canDig = false;
                        currentTile.canDropItem = false;
                        //音效
                        //EventHandler.CallPlaySoundEvent(SoundName.Hoe);
                        break;
                    case ItemType.WaterTool:
                        SetWaterGround(currentTile);
                        currentTile.daysSinceWatered = 0;
                        //音效
                        //EventHandler.CallPlaySoundEvent(SoundName.Water);
                        break;
                    //        case ItemType.BreakTool:
                    //        case ItemType.ChopTool:
                    //            //执行收割方法
                    //            currentCrop?.ProcessToolAction(itemDetails, currentCrop.tileDetails);
                    //            break;
                    case ItemType.CollectTool:
                        Crop currentCrop = GetCropObject(mouseWorldPos);
                        //执行收割方法
                        currentCrop.ProcessToolAction(itemDetails, currentTile);
                        //EventHandler.CallPlaySoundEvent(SoundName.Basket);
                        break;
                        //        case ItemType.ReapTool:
                        //            var reapCount = 0;
                        //            for (int i = 0; i < itemsInRadius.Count; i++)
                        //            {
                        //                EventHandler.CallParticleEffectEvent(ParticleEffectType.ReapableScenery, itemsInRadius[i].transform.position + Vector3.up);
                        //                itemsInRadius[i].SpawnHarvestItems();
                        //                Destroy(itemsInRadius[i].gameObject);
                        //                reapCount++;
                        //                if (reapCount >= Settings.reapAmount)
                        //                    break;
                        //            }
                        //            EventHandler.CallPlaySoundEvent(SoundName.Reap);
                        //            break;

                        //        case ItemType.Furniture:
                        //            //在地图上生成物品 ItemManager
                        //            //移除当前物品（图纸）InventoryManager
                        //            //移除资源物品 InventoryManger
                        //            EventHandler.CallBuildFurnitureEvent(itemDetails.itemID, mouseWorldPos);
                        //            break;
                }

                UpdateTileDetails(currentTile);
            }
        }
        /// <summary>
        /// 显示挖坑瓦片
        /// </summary>
        /// <param name="tile"></param>
        private void SetDigGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.gridX, tile.gridY, 0);
            if (digTileMap != null)
            {
                digTileMap.SetTile(pos, digTile);
            }
        }
        /// <summary>
        /// 显示浇水瓦片
        /// </summary>
        /// <param name="tile"></param>
        private void SetWaterGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.gridX, tile.gridY, 0);
            if (waterTileMap != null)
            {
                waterTileMap.SetTile(pos, waterTile);
            }
        }
        /// <summary>
        /// 更新瓦片信息
        /// </summary>
        /// <param name="tileDetails"></param>
        public void UpdateTileDetails(TileDetails tileDetails)
        {
            string key = tileDetails.gridX + "x" + tileDetails.gridY + "y" + SceneManager.GetActiveScene().name;
            if (tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key] = tileDetails;
            }
            else
            {
                tileDetailsDict.Add(key, tileDetails); 
            }
        }

        private void RefreshMap()
        {
            if (digTileMap != null)
                digTileMap.ClearAllTiles();
            if(waterTileMap != null)
                waterTileMap.ClearAllTiles();

            foreach(Crop crop in FindObjectsOfType<Crop>())
            {
                Destroy(crop.gameObject);
            }

            DisplayMap(SceneManager.GetActiveScene().name);
        }
        private void DisplayMap(string sceneName)
        {
            foreach (var tile in tileDetailsDict)
            {
                var key = tile.Key;
                var tileDetails = tile.Value;
                if(key.Contains(sceneName))
                {
                    if (tileDetails.daysSinceDug > -1) 
                        SetDigGround(tileDetails);
                    if(tileDetails.daysSinceWatered > -1)
                        SetWaterGround(tileDetails);
                    //种子
                    if (tileDetails.seedItemID > -1)
                        EventHeadler.CallPlantSeedEvent(tileDetails.seedItemID,tileDetails);
                }
            }
        }

        public Crop GetCropObject(Vector3 mouseWorldPos)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPos);

            Crop currentCrop = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Crop>())
                    currentCrop = colliders[i].GetComponent<Crop>();
            }
            return currentCrop;
        }

        public bool HaveReapableItemsInRadius(ItemDetails tool)
        {
            itemsInRadius = new List<ReapItem>();
            Collider2D[] colliders=new Collider2D[20];
            Physics2D.OverlapCircleNonAlloc(Input.mousePosition, tool.itemUseRadius, colliders);
            if(colliders.Length > 0)
            {
                for(int i=0;i<colliders.Length;i++)
                {
                    if (colliders[i] != null)
                    {
                        if (colliders[i].GetComponent<ReapItem>())
                        {
                            var item = colliders[i].GetComponent<ReapItem>();
                            itemsInRadius.Add(item);
                        }

                    }
                }
            }
        }
        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;
            foreach (var tile in tileDetailsDict)
            {
                if (tile.Value.daysSinceWatered > -1)
                {
                    tile.Value.daysSinceWatered = -1;
                }
                if(tile.Value.daysSinceDug > -1)
                {
                    tile.Value.daysSinceDug++;
                }
                if (tile.Value.daysSinceDug > 5 && tile.Value.seedItemID == -1)
                {
                    tile.Value.daysSinceDug = -1;
                    tile.Value.canDig = true;
                    tile.Value.growthDays = -1;
                }
                if(tile.Value.seedItemID != -1)
                {
                    tile.Value.growthDays++;
                }
            }
            RefreshMap();
        }
    }
    
}
