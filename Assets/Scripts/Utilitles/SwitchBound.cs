using Cinemachine;
using UnityEngine;

public class SwitchBound : MonoBehaviour
{
    private void OnEnable()
    {
        EventHeadler.AfterSceneLoadedEvent += SwitchConfinerShape;
    }

    private void OnDisable()
    {
        EventHeadler.AfterSceneLoadedEvent -= SwitchConfinerShape;
    }
    private void SwitchConfinerShape()
    {
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();

        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();

        confiner.m_BoundingShape2D = confinerShape;
        //运行时切换要调用清除缓存
        confiner.InvalidatePathCache();
    }


}
