using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fish", menuName = "Fish")]
public class Fishes : ScriptableObject
{
    // 鱼的基本属性
    public string fishSpecies; // 鱼的种类
    public string fishName; //鱼的名字
    public int fishWeight; //鱼的重量
    public Sprite sprite; // 鱼的外观

}
