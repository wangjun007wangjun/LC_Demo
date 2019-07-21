using System;
using BaseFramework;
using DG.Tweening;
using UnityEngine;

public class MoveView : MonoBehaviour
{
    public enum Status
    {
        In,
        Out,
        Inning,
        Outting,
    }

    public Vector3 moveOffset;
    public float moveTime;
    public float delay;

    public bool hasToken { get; set; } = true;

    public Status status = Status.Out;

    private Vector3 _originalPosition;

    private Tween _tween;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = (RectTransform) transform;
        _originalPosition = _rectTransform.anchoredPosition.ToV3();
    }

    public void PlayIn(Action finish = null, float customDelay = 0)
    {
        _tween?.Kill();
        _tween = _rectTransform.DOAnchorPos(_originalPosition + moveOffset, moveTime)
                               .OnStart(() => status = Status.Inning)
                               .SetDelay(customDelay.Eq(0) ? delay : customDelay)
                               .SetEase(Ease.OutQuad)
                               .OnComplete(() =>
                                           {
                                               finish?.Invoke();
                                               status = Status.In;
                                           });
    }

    public void PlayOut(Action finish = null, float customDelay = 0)
    {
        if (!hasToken) return;
        _tween?.Kill();
        _tween = _rectTransform.DOAnchorPos(_originalPosition, moveTime)
                               .SetDelay(customDelay.Eq(0) ? delay : customDelay)
                               .SetEase(Ease.InQuad)
                               .OnStart(() => status = Status.Outting)
                               .OnComplete(() =>
                                           {
                                               finish?.Invoke();
                                               status = Status.Out;
                                           });
    }

    public void Stop(bool atIn)
    {
        _tween?.Kill();
        _rectTransform.anchoredPosition = _originalPosition + (atIn ? moveOffset : Vector3.zero);
        status = atIn ? Status.In : Status.Out;
    }
}