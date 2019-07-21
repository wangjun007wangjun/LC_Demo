using UnityEngine;

public class LevelEmptyItemView : ALevelBaseItemView
{
    public LevelEmptyWrapBean bean =>  _bean as LevelEmptyWrapBean;

    private RectTransform _rectTransform;

    public RectTransform rectTransform
    {
        get
        {
            _rectTransform = _rectTransform ?? GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    public override void Init(ALevelItemViewWrapBean bean)
    {
        base.Init(bean);

        rectTransform.sizeDelta = new Vector2(bean.size, rectTransform.sizeDelta.y);
    }
}