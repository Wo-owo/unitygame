using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class WeatherManager : MonoBehaviour {
    
    public GameObject rainPrefab;
    public Vector2 pos1,pos2;
    public static WeatherManager instance;
    public float yoffset;
    public float fadeSpeed = 1.5f;          // Speed that the screen fades to and from black.
    public bool fadeInOnStart = false;      // Whether or not the scene should fade in on start.

    private Color startColor;               // The color the object starts with.
    private Color endColor = Color.clear;   // The color the object ends with (transparent).
    private void Start() {
        instance= this;
        pos1 = rainPrefab.transform.position;
        //pos2 = rainPrefab2.transform.position;
        startColor = rainPrefab.GetComponent<Image>().color;
    }
    public void StartRain(bool _isRain){
        //if(_isRain){
            rainPrefab.transform.position=pos1;
            
            
            if(_isRain){
                rainPrefab.SetActive(_isRain);
                StartCoroutine(FadeIn());

            }
            else{
                StartCoroutine(FadeOut());
            }
            
    }
    /// <summary>
    /// 改变天气
    /// </summary>
    public void WeatherChange()
    {
        GameManager.instance.isRain=!GameManager.instance.isRain;
        StartRain(GameManager.instance.isRain);
        
    }
    IEnumerator FadeIn()
    {
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime * fadeSpeed;
            rainPrefab.GetComponent<Image>().color = Color.Lerp(endColor, startColor, t);
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            rainPrefab.GetComponent<Image>().color = Color.Lerp(endColor, startColor, t);
            yield return null;
        }
        rainPrefab.SetActive(false);
    }

}