using System;
using BaseFramework;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FillLetterView : MonoRecycleItem<FillLetterView>
{
    public float shakePreDurationTime = 0.04f;
    public float shakeAngle = 15;
    public int rowIndex;//自身的行索引
    public static FillLetterView Instance(FillLetterView prefab,
                                          Transform parentTransform,
                                          char letter,
                                          int rowIndex)
    {
        FillLetterView fillLetterView = PoolHelper.Create(prefab);
        fillLetterView.transform.SetParent(parentTransform, false);

        fillLetterView.letter = letter;
        fillLetterView.rowIndex = rowIndex;
        return fillLetterView;
    }

    public Text text;

    private char _letter;

    public char letter
    {
        get => _letter;
        set
        {
            _letter = value;
            text.text = value.ToString();
        }
    }

    private RectTransform _rectTransform;

    public RectTransform rectTransform
    {
        get
        {
            _rectTransform = _rectTransform ?? GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    private Tween _shakeTween;
    private Tween _flyTween;

    public void Shake(float delay)
    {
        if (_shakeTween == null)
        {
            _shakeTween = DOTween.Sequence()
                            .Append(transform.DOLocalRotate(new Vector3(0, 0, -shakeAngle), shakePreDurationTime))
                            .Append(transform.DOLocalRotate(new Vector3(0, 0, shakeAngle * 0.5f),
                                                            shakePreDurationTime * 2))
                            .Append(transform.DOLocalRotate(new Vector3(0, 0, -shakeAngle * 0.25f),
                                                            shakePreDurationTime * 2))
                            .Append(transform.DOLocalRotate(Vector3.zero, shakePreDurationTime))
                            .SetDelay(delay)
                            .OnKill(() => transform.localEulerAngles = Vector3.zero);
        }
        else
        {
            transform.localEulerAngles = Vector3.zero;
            _shakeTween.SetDelay(delay);
            _shakeTween.Restart();
        }
    }

    public void Fly(SelectedBlockView selectedBlockView, float duration, float delay, Action onFinished = null)
    {
        Vector3 startPosition = selectedBlockView.rectTransform.position;
        Vector3 startLocalScale = Vector3.one * selectedBlockView.rectTransform.rect.height / rectTransform.rect.height;
        Vector3 endPosition = transform.position;
        Vector3 endLocalScale = Vector3.one;

        transform.position = startPosition;
        transform.localScale = startLocalScale;
        if (_flyTween == null)
        {
            _flyTween = DOTween.Sequence()
                               .Append(transform.DOMove(endPosition, duration).SetEase(Ease.InQuad))
                               .Insert(0, transform.DOScale(endLocalScale, duration).SetEase(Ease.InOutBack))
                               .SetDelay(delay)
                               .OnKill(() =>
                                       {
                                           transform.position = endPosition;
                                           transform.localScale = endLocalScale;
                                       })
                               .OnComplete(() => onFinished?.Invoke());
        }
        else
        {
            _flyTween.SetDelay(delay);
            _flyTween.Restart();  
        }
    }

    public void Fly(RectTransform rectTransform, float duration, float delay, Action onFinished = null)
    {
        Vector3 startPosition = rectTransform.position;
        Vector3 startLocalScale = Vector3.one * rectTransform.rect.height / rectTransform.rect.height;
        Vector3 endPosition = transform.position;
        Vector3 endLocalScale = Vector3.one;

        transform.position = startPosition;
        transform.localScale = startLocalScale;
        if (_flyTween == null)
        {
            _flyTween = DOTween.Sequence()
                               .Append(transform.DOMove(endPosition, duration).SetEase(Ease.InQuad))
                               .Insert(0, transform.DOScale(endLocalScale, duration).SetEase(Ease.InOutBack))
                               .SetDelay(delay)
                               .OnKill(() =>
                                       {
                                           transform.position = endPosition;
                                           transform.localScale = endLocalScale;
                                       })
                               .OnComplete(() => onFinished?.Invoke());
        }
        else
        {
            _flyTween.SetDelay(delay);
            _flyTween.Restart();
        }
    }

    public override void OnRecycle()
    {
        base.OnRecycle();
        
        _flyTween?.Kill();
        _shakeTween?.Kill();
    }
}