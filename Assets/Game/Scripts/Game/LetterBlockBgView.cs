using BaseFramework;
using UnityEngine;
using UnityEngine.UI;

public class LetterBlockBgView : MonoRecycleItem<LetterBlockBgView>
{
    public Vector3 localPositionOffset = new Vector3(0, -5, 0);
    
    public static LetterBlockBgView Instance(LetterBlockBgView prefab,
                                             Transform parentTransform,
                                             Color color)
    {
        LetterBlockBgView letterBlockBgView = PoolHelper.Create(prefab);
        letterBlockBgView.transform.SetParent(parentTransform, false);
        letterBlockBgView.color = color;
        letterBlockBgView.Active();

        return letterBlockBgView;
    }

    public Vector3 position
    {
        set
        {
            transform.position = value;
            transform.localPosition = transform.localPosition + localPositionOffset;
        }
    }

    public Vector3 localScale
    {
        set => transform.localScale = value;
    }

    private Color color
    {
        set => image.color = value;
    }

    private Image _image;
    private Image image
    {
        get
        {
            _image = _image ?? GetComponent<Image>();
            return _image;
        }
    }
}