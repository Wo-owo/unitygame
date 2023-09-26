using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    Scrollbar scrollbar;
    private void Awake()
    {
        scrollbar = GetComponent<Scrollbar>();
    }
    private void Start()
    {
        MiniGameManager.Instance.mProgressBar = this;
    }
    public float Size { get => scrollbar.size; set => scrollbar.size = value; }
    public Sprite Sprite { get => scrollbar.image.sprite; set => scrollbar.image.sprite = value; }
}
