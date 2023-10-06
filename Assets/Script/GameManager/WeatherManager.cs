using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
public class WeatherManager : MonoBehaviour {
    
    public GameObject rainPrefab;
    public Vector2 pos1,pos2;
    public static WeatherManager instance;
    public float yoffset;
    private void Start() {
        instance= this;
        pos1 = rainPrefab.transform.position;
        //pos2 = rainPrefab2.transform.position;
    }
    public void StartRain(bool _isRain){
        //if(_isRain){
            rainPrefab.transform.position=pos1;
            
            rainPrefab.SetActive(_isRain);
            
    }
    /// <summary>
    /// 改变天气
    /// </summary>
    public void WeatherChange()
    {
        
    }
}