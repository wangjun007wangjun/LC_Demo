using System.Collections;
using System.Collections.Generic;
using BaseFramework;
using BaseFramework.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelPassDialog : Dialog
{
   public Text LevelText;
   public Image cardImage;
   public Image cardOtherSide;
   public Button turnOverButton;
   public Image progressImage;
   public Text progressText;
   public Button nextButton;

   public Image GoldBox;
   private LevelBean _levelBean;

   protected override void Awake()
   {
       base.Awake();
      nextButton.onClick.AddListener(EnterNextLevel);
      turnOverButton.onClick.AddListener(TurnOverCard);
   }
   public void Init(LevelBean levelBean)
    {
        this._levelBean = levelBean;
        //todo 播放声音
        
        //todo 检测一个信封、主题是否结束
        LevelText.text ="Level " + (PlayerDataManager.levelInfo.currentLevelIndex+1);
        //todo 取得对应卡片

        int curLevelIndex = PlayerDataManager.levelInfo.currentLevelIndex;
        int curThemeIndex = PlayerDataManager.levelInfo.currentThemeIndex;
        
        float progress = (curLevelIndex+1) / (float)
                         LevelManager
                            .instance.GetEnvelope(curThemeIndex,
                                                  curLevelIndex).levelCount;
        progressImage.fillAmount = progress >= 1 ? 1 : progress;
        progressText.text = (curLevelIndex+1) + "/" +
                            LevelManager.instance.GetEnvelope(curThemeIndex,curLevelIndex).levelCount;

        if (progress >= 1)
        {
            progressImage
               .gameObject
               .transform
               .parent
               .Inactive();
            nextButton
               .gameObject
               .Inactive();
            //移动到屏幕中央
            GoldBox.gameObject.transform.DOLocalMove(new Vector3(0,0,0), 0.4f).OnComplete(() =>
                                                                                          {
                                                                                              GoldBox.GetComponent<Button>()
                                                                                                  .enabled = true;
                                                                                              GoldBox
                                                                                                 .GetComponent<Button>().onClick.AddListener(GiftBoxClick);
                                                                                              GoldBox
                                                                                                 .gameObject
                                                                                                 .transform
                                                                                                 .DOScale(new Vector3(1.2f, 1.2f, 1.2f),
                                                                                                          0.4f);

                                                                                          });
        }
    }

    private void EnterNextLevel()
    {
        //结算弹窗自身擦除式消失
        this.GetComponent<CanvasGroup>().DOFade(0, 0.4f).OnComplete(Close);        
        GameDialogManager.instance.ClearDialogPool();
        //test
        //LevelManager.instance.LevelPass(_levelBean);

        EventManager.instance.DispatchEvent(CrossPanelEvent.instance.LevelOnComplete());
   
        float percent =
            (BgController.instance._rectTransform.rect.width /
             (LevelManager.instance.GetTheme(_levelBean.id).levelCount)+LevelManager.instance.GetTheme(_levelBean.id).envelopeCount) /
            BgController.instance._rectTransform.rect.width;
       //操作盘消失
       GameController.GetInstance().letterPanel.BgImageHide(() =>
                                                            {
                                                                SceneHelper.instance.LoadScene(Const.Scene.GAME, 0,
                                                                                               null,
                                                                                               null);
                                                            });
       TaskHelper.Create<CoroutineTask>().Delay(0.5f).Do(() =>
                                                         {
                                                             EventManager
                                                                .instance
                                                                .DispatchEvent(new
                                                                                   LevelScrollChangeEvent(percent,
                                                                                                          true));
                                                         }).Execute();
    }

    private void TurnOverCard()
    {
        if (cardImage.gameObject.activeInHierarchy)
        {
            cardImage.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.4f).OnComplete(() =>
                                                                                      {
                                                                                          cardImage.Inactive();
                                                                                          cardOtherSide.Active();
                                                                                          cardOtherSide
                                                                                             .transform
                                                                                             .DOLocalRotate(new Vector3(0, 0, 0),
                                                                                                            0.4f);
                                                                                      });
        }
        else
        {
            cardOtherSide.transform.DOLocalRotate(new Vector3(0,90,0), 0.4f).OnComplete(()=>
                                                                                        {
                                                                                            cardOtherSide.Inactive();
                                                                                            cardImage.Active();
                                                                                            cardImage
                                                                                               .transform
                                                                                               .DOLocalRotate(new Vector3(0, 0, 0),
                                                                                                              0.4f);
                                                                                        });
        }
        
    }

    public void GiftBoxClick()
    {
        //todo 金币增加
        GoldBox.Inactive();
        nextButton
           .gameObject
           .Active();
    }
}
