using System;
using BaseFramework;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ProgressCutterBar : MonoBehaviour
{
    public bool canReduce;
    public float startProgress = 0;

    private Text _progressText;
    private ProgressCutterImage _progressImg;
    private Tween _tween;

    private void Awake()
    {
        _progressText = transform.Find("ProgressText")?.GetComponent<Text>();
        _progressImg = transform.Find("ProgressImg").GetComponent<ProgressCutterImage>();
        _progressImg.SetProgress(startProgress);
    }

    public void UpdateProgress(float progress, float time, string text = null, Action onProgressComplete = null)
    {
        this.Delay(0, () => {
            SetText(text);
            Progress(progress, time, false, onProgressComplete);
        });
    }
    
    public void UpdateProgress(float progress, float time, bool autoText = false, Action onProgressComplete = null)
    {
        this.Delay(0, () => {
                          Progress(progress, time, autoText, onProgressComplete);
                      });
    }

    private void SetText(string text)
    {
        if (_progressText != null)
        {
            _progressText.text = text.Any() ? text : string.Empty;
        }
    }

    private void Progress(float progress, float time, bool autoText, Action onProgressComplete)
    {
        progress = Mathf.Clamp01(progress);

        if (!canReduce && progress < _progressImg.GetProgress())
        {
            return;
        }

        _tween?.Kill();
        _tween = DOTween.To(_progressImg.GetProgress,
                            (v) =>
                            {
                                _progressImg.SetProgress(v);
                                SetText((int)(v * 100) + "%");
                            }, 
                            progress,
                            time)
               .SetEase(Ease.Linear)
               .OnComplete(() => {
                   if (progress.Eq(1))
                   {
                       onProgressComplete?.Invoke();
                   }
               });
    }
}
