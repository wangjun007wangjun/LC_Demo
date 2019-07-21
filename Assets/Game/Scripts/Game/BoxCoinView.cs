using BaseFramework;
using UnityEngine;

public class BoxCoinView : MonoRecycleItem<BoxCoinView>
{
    public static BoxCoinView Instance(BoxCoinView prefab,
                                       Transform parentTransform)
    {
        BoxCoinView boxCoinView = PoolHelper.Create(prefab);
        boxCoinView.transform.SetParent(parentTransform, false);

        return boxCoinView;
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

    private Vector2 size => rectTransform.rect.size;
    private Vector2 position => transform.position;

    public void Collect()
    {
        EventManager.instance.DispatchEvent(new CoinCollectEvent(size, position));

        Dispose();
    }
}