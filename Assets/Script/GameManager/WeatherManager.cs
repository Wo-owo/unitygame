using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
public class WeatherManager : MonoBehaviour {
    
    public GameObject rainPrefab1,rainPrefab2;
    public Vector2 pos1,pos2;
    public static WeatherManager instance;
    public void StartRain(bool _isRain){
        //if(_isRain){
            rainPrefab1.transform.position=pos1;
            rainPrefab2.transform.position = pos2;

            rainPrefab1.SetActive(_isRain);
            rainPrefab2.SetActive(_isRain);
        
        
        //}
        

    }

}