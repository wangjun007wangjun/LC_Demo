using BaseFramework;
using UnityEngine;
using UnityEngine.UI;

public class LevelEnvelopeItemView : ALevelBaseItemView
{
    public Sprite closeSprite;
    public Sprite openSprite;
    public Image image;
    
    public LevelContentWrapBean bean => _bean as LevelContentWrapBean;

    public EnvelopeBean envelopeBean { get; set; }

    private bool _isSelected;
    public override bool isSelected
    {
        protected get => _isSelected;
        set
        {
            _isSelected = value;
            
            if (levelStatus == LevelStatus.Lock)
                return;

            image.sprite = value ? openSprite : closeSprite;
        }
    }
    
    private LevelStatus _levelStatus;
    public override LevelStatus levelStatus
    {
        get => _levelStatus;
        set
        {
            _levelStatus = value;

            if(envelopeBean == null) return;
            
            switch (value)
            {
                case LevelStatus.Current:
                    image.color = envelopeBean.color;
                    break;
                case LevelStatus.Unlock:
                    image.color = envelopeBean.color;
                    break;
                case LevelStatus.Lock:
                    image.color = Color.gray;
                    break;
            }
        }
    }

    public override void Init(ALevelItemViewWrapBean bean)
    {
        base.Init(bean);
        this.envelopeBean = this.bean.bean as EnvelopeBean;

        UpdateStatus();
    }

    public override void UpdateStatus()
    {
        base.UpdateStatus();
        
        levelStatus = LevelManager.instance.GetEnvelopStatus(envelopeBean);
    }
    
    protected override void OnClick()
    {
        if (!isSelected)
        {
            base.OnClick();
            return;
        }

        EventManager.instance.DispatchEvent(new SelectedEnvelopeClickEvent(this));
    }
}