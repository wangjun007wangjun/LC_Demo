using BaseFramework;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LetterBlockView : MonoRecycleItem<LetterBlockView>
{
    public static LetterBlockView Create(LetterBlockView letterBlockViewPrefab,
                                         Transform parentTransform,
                                         char letter,
                                         Vector3 localPosition,
                                         float localScale)
    {
        LetterBlockView letterBlockView = PoolHelper.Create(letterBlockViewPrefab);
        letterBlockView.Name(letterBlockViewPrefab.name.AddSuffix("_") + letter);
        letterBlockView.letter = letter;
        letterBlockView.RandomAngle();
        letterBlockView.LocalScale(Vector3.one * localScale);
        letterBlockView.PlayShowAnimation();
        letterBlockView.transform.Parent(parentTransform, false).LocalPosition(localPosition).Active();

        return letterBlockView;
    }

    public float selectScale = 1.1f;
    private Vector3 _originScale;
    private Vector3 _originRotation;
    private Tween _tween;

    private bool _isSelected;

    public bool isSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;

            if (value)
                StopTween();
            else
                _letterBlockBgView?.Dispose();
            transform.localScale = _originScale * (value ? selectScale : 1);
        }
    }

    private char _letter;

    public char letter
    {
        get => _letter;
        set
        {
            _letter = value;
            _text.text = _letter.ToString();
        }
    }

    private LetterBlockBgView _letterBlockBgView;

    public LetterBlockBgView letterBlockBgView
    {
        set
        {
            _letterBlockBgView = value;

            _letterBlockBgView.position = rectTransform.position;
            _letterBlockBgView.localScale = rectTransform.localScale;
        }
    }

    public RectTransform rectTransform { get; private set; }

    private Text _text;

    private void Awake()
    {
        rectTransform = transform as RectTransform;
        _text = GetComponent<Text>();
    }

    public void RandomAngle(float range = 10)
    {
        _originRotation = new Vector3(0, 0, Random.Range(-range, range));
        transform.localEulerAngles = _originRotation;
    }

    public void PlayShowAnimation()
    {
        transform.DOScale(Vector3.zero, 0.5f).From().SetEase(Ease.OutBack);
    }

    public void LocalScale(Vector3 scale)
    {
        _originScale = scale;
        transform.LocalScale(scale);
    }

    private void StopTween()
    {
        if (_tween != null && _tween.IsActive() && _tween.IsPlaying())
        {
            _tween.Kill();
        }
    }

    public void Shake()
    {
        StopTween();
        _tween = DOTween.Shake(() => transform.localEulerAngles,
                               (v) => transform.localEulerAngles = v,
                               0.5f, new Vector3(0, 0, 20), 20)
                        .OnKill(() => transform.localEulerAngles = _originRotation);
    }

    public void Jelly()
    {
        StopTween();
        // 选中时已经放大
        _tween = DOTween.Sequence()
                        .Append(transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.Linear))
                        .Append(transform.DOScale(Vector3.one * (1 + selectScale) / 2, 0.1f)
                                         .SetEase(Ease.Linear))
                        .Append(transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.Linear))
                        .OnKill(() => transform.localScale = Vector3.one);
    }

    public override void OnRecycle()
    {
        StopTween();
        isSelected = false;
        base.OnRecycle();
    }

    public void Hide()
    {
        transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(Dispose);
    }

    // for debug
    private void OnDrawGizmosSelected()
    {
        Rect rect = GetComponent<RectTransform>().GetWordRect();
        // rect = new Rect(rect.center - rect.size * 0.8f / 2, rect.size * 0.8f);
        rect = new Rect(rect.center - rect.size * 0.5f, rect.size);
        Gizmos.DrawLine(new Vector3(rect.xMin, rect.yMax, 90), new Vector3(rect.xMin, rect.yMin, 90));
        Gizmos.DrawLine(new Vector3(rect.xMin, rect.yMin, 90), new Vector3(rect.xMax, rect.yMin, 90));
        Gizmos.DrawLine(new Vector3(rect.xMax, rect.yMin, 90), new Vector3(rect.xMax, rect.yMax, 90));
        Gizmos.DrawLine(new Vector3(rect.xMax, rect.yMax, 90), new Vector3(rect.xMin, rect.yMax, 90));
    }
}