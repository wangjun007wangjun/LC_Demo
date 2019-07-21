using System.Net.NetworkInformation;
using BaseFramework;
using DG.Tweening;
using UnityEngine;

public class ThemeBg : MonoBehaviour
{
    public int index;
    
    private RectTransform _rectTransform;
    private float _scrollSize;

    private float jumpRange = 0.15f;
    private float jumpCenter = 0f;

    private float lastPosX = 0;
    private void Awake()
    {
        _rectTransform = transform as RectTransform;
        _scrollSize = _rectTransform.rect.width - ScreenSizeManager.instance.GetCanvasRealSize().x;

        EventManager.instance.RegistEvent<LevelScrollChangeEvent>(OnLevelScrollChange);
    }

    private void OnDestroy()
    {
        EventManager.instance.UnRegistEvent<LevelScrollChangeEvent>(OnLevelScrollChange);
    }

    private void OnLevelScrollChange(LevelScrollChangeEvent @event)
    {
        Debug.Log("event.item:"+@event.item);
        float posY = 0;
        float dis = Mathf.Abs(@event.item - jumpCenter);
        if (dis < jumpRange)
        {
            posY = (1 - dis / jumpRange) * 15 * index;
        }

        if (@event.levelPass)
        {
            if (SceneHelper.instance.lastSceneName == Const.Scene.GAME)
            {
                _rectTransform.DOAnchorPos(new Vector2(lastPosX-@event.item * _scrollSize, posY), 0.4f).OnComplete(() =>
                                                                                                                   {
                                                                                                                       lastPosX
                                                                                                                           = _rectTransform
                                                                                                                            .anchoredPosition
                                                                                                                            .x;
                                                                                                                   });
            }
            else if (SceneHelper.instance.lastSceneName == Const.Scene.LEVEL)
            {
                //直接设置位置
                _rectTransform.anchoredPosition = new Vector2(lastPosX,0);
                lastPosX = _rectTransform.anchoredPosition.x;
            }
        }
        else
        {
            _rectTransform.anchoredPosition = new Vector2(-@event.item * _scrollSize, posY);
            lastPosX = _rectTransform.anchoredPosition.x;
        }
    }

    private void OnLevelToGame()
    {
        _rectTransform.anchoredPosition = new Vector2(0,0);
    }
}