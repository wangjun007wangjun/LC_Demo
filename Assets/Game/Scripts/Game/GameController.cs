using System;
using BaseFramework;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController _instance;

    public static GameController GetInstance()
    {
        return _instance;
    }

    public CrossPanel crossPanel;
    public LetterPanel letterPanel;
    public SelectedPanel selectedPanel;
    
    private LevelBean _levelBean;

    public RectTransform Bg;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // _levelBean = LevelManager.instance.GetLevel(PlayerDataManager.levelInfo.currentThemeIndex,
        //                                             PlayerDataManager.levelInfo.currentEnvelopeIndex,
        //                                             PlayerDataManager.levelInfo.currentLevelIndex);
        _levelBean = LevelManager.instance.GetLevel(0,0,0);
    }

    private void Start()
    {
        Ready(null);
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
    private void Ready(TransitionBase transition)
    {
        // ReSharper disable once DelegateSubtraction
       // BlindTransition.instance.onExitCallback -= Ready;
        
        //上方资源出现
        UiMoveController.instance.TopUiMoveIn(()=> 
                                                  crossPanel.Init(_levelBean,()=>
                                                                                  UiMoveController.instance.LeftAndRightUiMoveIn(()=> 
                                                                                                                                     letterPanel.Init(_levelBean))));
    }

    public void WillComplete()
    {
        // 打开通关弹窗
        //UI移出，结算弹窗弹出
        UiMoveController.instance.Hide(() =>
                                       {
                                           this.Delay(0.7f).Do(() =>
                                                               {
                                                                   LevelPassDialog levelPassDialog =
                                                                       GameDialogManager.instance.Open("LevelPassDialog") as LevelPassDialog;
                                                                   if (levelPassDialog != null)
                                                                       levelPassDialog.Init(_levelBean);
                                                               }).Execute();
                                           
                                       });
    }
}