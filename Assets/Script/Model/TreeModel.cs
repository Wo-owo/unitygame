using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeModel : MonoBehaviour
{
    private SpriteRenderer _sprite;
    /// <summary>
    /// 树渲染组件
    /// </summary>
    public SpriteRenderer Sprite { get => _sprite; }
    /// <summary>
    /// 树等级
    /// </summary>
    public int TreeLevel = 0;
    /// <summary>
    /// 日浇水次数
    /// </summary>
    public int TreeFrequency;
    /// <summary>
    /// 耗水量
    /// </summary>
    public float TreeLitre;
    /// <summary>
    /// 绿化值
    /// </summary>
    public float GreenValue;
    // Start is called before the first frame update
    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }
}
