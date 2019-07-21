using System.Collections;
using System.Collections.Generic;
using BaseFramework;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private static LevelController _instance;

    public static LevelController GetInstance()
    {
        return _instance;
    } 
    
    public LevelListPanel levelListPanel;
    public CardPanel cardPanel;

    private ThemeBean _themeBean;
    
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

        _themeBean = LevelManager.instance.GetTheme(PlayerDataManager.levelInfo.currentThemeIndex);
        
        BlindTransition.instance.onExitCallback += Ready;
        
        EventManager.instance.RegistEvent<SelectedEnvelopeClickEvent>(OnEnvelopeClick);
    }

    private void OnDestroy()
    {
        EventManager.instance.UnRegistEvent<SelectedEnvelopeClickEvent>();

        if (_instance == this)
            _instance = null;
    }

    private void Ready(TransitionBase transition)
    {
        // ReSharper disable once DelegateSubtraction
        BlindTransition.instance.onExitCallback -= Ready;
        
        levelListPanel.Init(_themeBean);
    }

    private void OnEnvelopeClick(SelectedEnvelopeClickEvent @event)
    {
        cardPanel.Show(@event.itemView);
    }
}