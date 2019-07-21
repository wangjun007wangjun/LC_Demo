using BaseFramework;
using UnityEngine;
using UnityEngine.UI;

public class SelectedBlockView : MonoRecycleItem<SelectedBlockView>
{
    public static SelectedBlockView Instance(SelectedBlockView prefab,
                                             Transform parentTransform,
                                             char letter)
    {
        SelectedBlockView selectedBlockView = PoolHelper.Create(prefab);
        selectedBlockView.Name(prefab.name + "_" + letter)
                     .transform.Parent(parentTransform, false)
                     .Active();

        selectedBlockView.letter = letter;

        return selectedBlockView;
    }
    
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

    private Text _text;

    private Text text
    {
        get
        {
            _text = _text ?? GetComponent<Text>();
            return _text;
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
}