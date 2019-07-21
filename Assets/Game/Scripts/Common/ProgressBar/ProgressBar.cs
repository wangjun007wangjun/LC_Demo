using System;
using BaseFramework;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    public bool canReduce;

    public int contentMinWidth;
    public int dtWidth = 0;

    private TextMeshProUGUI _progressText;
    private Image _progressImg;

    private bool _initFinished;
    private float _maxWidth = -1;
    private RectTransform _cacheImageRectTransform;
    private float _cacheY;
    private Tween _tween;

    private void Init()
    {
        _progressText = transform.Find("ProgressText")?.GetComponent<TextMeshProUGUI>();
        _progressImg = transform.Find("ProgressImg").GetComponent<Image>();
        _cacheImageRectTransform = _progressImg.rectTransform;
        _cacheY = _cacheImageRectTransform.sizeDelta.y;
        _maxWidth = ((RectTransform) transform).rect.size.x - dtWidth;
        _initFinished = true;
    }

    public void UpdateProgress(float progress, float time, string text = null, Action onProgressComplete = null)
    {
        if (!_initFinished)
        {
            TaskHelper.Create<CoroutineTask>().Delay(0).Do(() => {
                if (this != null)
                {
                    Init();
                    SetText(text);
                    Progress(progress, time, onProgressComplete);
                }
            }).Execute();
        }
        else
        {
            SetText(text);
            Progress(progress, time, onProgressComplete);
        }
    }
    
    private void SetText(string text)
    {
        if (_progressText != null)
        {
            _progressText.text = text.Any() ? text : string.Empty;
        }
    }

    private void Progress(float progress, float time, Action onProgressComplete)
    {
        progress = Mathf.Clamp01(progress);

        float targetWidth = progress * _maxWidth;

        if (targetWidth < contentMinWidth)
        {
            targetWidth = 0;
            _progressImg.enabled = false;
        }
        else
        {
            _progressImg.enabled = true;
        }

        if (!canReduce && targetWidth < GetCurrentWidth()) return;

        _tween?.Kill();
        _tween = DOTween.To(GetCurrentWidth, SetCurrentWidth, targetWidth, time)
               .SetEase(Ease.Linear)
               .OnComplete(() => {
                   if (progress.Eq(1))
                   {
                       onProgressComplete?.Invoke();
                   }
               });
    }

    private float GetCurrentWidth()
    {
        return _progressImg == null ? 0 : _cacheImageRectTransform.sizeDelta.x;
    }

    private void SetCurrentWidth(float width)
    {
        _cacheImageRectTransform.sizeDelta = new Vector2(width, _cacheY);
    }
}
