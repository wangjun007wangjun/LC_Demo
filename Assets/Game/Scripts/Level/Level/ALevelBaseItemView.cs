using BaseFramework;
using UnityEngine;
using UnityEngine.UI;

public abstract class ALevelBaseItemView : MonoBehaviour
{
    public Transform contentTransform { get; private set; }

    public virtual bool isSelected { protected get; set; }

    public virtual LevelStatus levelStatus { get; set; }

    protected ALevelItemViewWrapBean _bean;
    
    private Button _button;

    protected virtual void Awake()
    {
        contentTransform = transform.Find("Content");
        _button = transform.Find("Content")?.GetComponent<Button>();
        _button?.onClick.AddListener(OnClick);
    }

    public virtual void Init(ALevelItemViewWrapBean bean)
    {
        this._bean = bean;
    }

    public virtual void UpdateStatus()
    {
        
    }

    protected virtual void OnClick()
    {
        EventManager.instance.DispatchEvent(new LevelBaseItemClickEvent(_bean.index));
    }
}