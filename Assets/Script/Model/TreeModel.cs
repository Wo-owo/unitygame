using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeModel : MonoBehaviour
{
    private SpriteRenderer _sprite;
    /// <summary>
    /// ����Ⱦ���
    /// </summary>
    public SpriteRenderer Sprite { get => _sprite; }
    /// <summary>
    /// ���ȼ�
    /// </summary>
    public int TreeLevel = 0;
    /// <summary>
    /// �ս�ˮ����
    /// </summary>
    public int TreeFrequency;
    /// <summary>
    /// ��ˮ��
    /// </summary>
    public float TreeLitre;
    /// <summary>
    /// �̻�ֵ
    /// </summary>
    public float GreenValue;
    // Start is called before the first frame update
    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }
}
