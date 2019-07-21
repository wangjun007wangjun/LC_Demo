using System.Collections;
using System.Collections.Generic;
using BaseFramework;
using DG.Tweening;
using UnityEngine;

public class BgController : MonoSingleton<BgController>
{
    public RectTransform _rectTransform;
    private float width;

    public float offsetPercentInLevel = 0f;
    
    public Vector2 lastPosition = Vector2.zero;

    protected override void Awake()
    {
        base.Awake();
        _rectTransform =transform.Find("Panel11").GetComponent<RectTransform>();
        
        width = _rectTransform.rect.width;
    }
}
