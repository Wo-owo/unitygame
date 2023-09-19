using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏主程序控制
/// </summary> 
public class GameManager : MonoBehaviour
{
    public static GameManager instance;//单例

    

    private void Awake() {
        if(instance != null){
            
        }
        instance = this;

    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        //游戏流程

        //沙尘暴控制状态机

        //等等之类
    }


}
