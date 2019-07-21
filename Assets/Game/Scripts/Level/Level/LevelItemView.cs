using BaseFramework;
using UnityEngine;
using UnityEngine.UI;

public class LevelItemView : ALevelBaseItemView
{
    public Sprite unlockSprite;
    public Sprite currentSprite;
    public Sprite lockSprite;
    public Image image;
    public Text text;
    public LevelContentWrapBean bean => _bean as LevelContentWrapBean;

    private LevelStatus _levelStatus;
    public override LevelStatus levelStatus
    {
        get => _levelStatus;
        set
        {
            _levelStatus = value;

            if(_levelBean == null) return;
            
            switch (value)
            {
                case LevelStatus.Current:
                    image.sprite = currentSprite;
                    image.color = _levelBean.color;
                    text.color = _levelBean.color;
                    break;
                case LevelStatus.Unlock:
                    image.sprite = unlockSprite;
                    image.color = _levelBean.color;
                    text.color = Color.black;
                    break;
                case LevelStatus.Lock:
                    image.sprite = lockSprite;
                    image.color = Color.white;
                    text.color = Color.white;
                    break;
            }
        }
    }

    private LevelBean _levelBean;

    public override void Init(ALevelItemViewWrapBean bean)
    {
        base.Init(bean);
        _levelBean = this.bean.bean as LevelBean;

        text.text = (_levelBean.idInEnvelope + 1).ToString();

        UpdateStatus();
    }

    public override void UpdateStatus()
    {
        base.UpdateStatus();
        
        LevelStatus tempStatus = LevelManager.instance.GetLevelStatus(_levelBean);
        if (tempStatus == LevelStatus.Current)
        {
            EnvelopeBean envelopeBean = LevelManager.instance.GetEnvelope(_levelBean);
            if (envelopeBean.id > PlayerDataManager.levelInfo.unlockEnvelopeId)
            {
                tempStatus = LevelStatus.Lock;
            }
        }
        levelStatus = tempStatus;
    }

    protected override void OnClick()
    {     
        if (!isSelected)
        {
            base.OnClick();
            return;
        }

        //LevelManager.instance.LevelPass(_levelBean);
        //test
        //BgController.instance._rectTransform;
        //记录当前背景位置
        EventManager
           .instance
           .DispatchEvent(new
                              LevelScrollChangeEvent(0f,
                                                     true));
        SceneHelper.instance.LoadScene(Const.Scene.GAME,0,
                                       BlindTransition.instance.Enter,
                                       BlindTransition.instance.Exit);
        
        EventManager.instance.DispatchEvent(new LevelBaseItemClickEvent(bean.index + 1));
    }
}