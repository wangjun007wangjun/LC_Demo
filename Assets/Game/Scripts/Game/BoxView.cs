using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BaseFramework;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BoxView : MonoRecycleItem<BoxView>
{
    public Leaf leafPrefab;
    
    public BoxCoinView boxCoinViewPrefab;
    public BoxCrossView boxCrossViewPrefab;
    public FillLetterView fillLetterViewPrefab;
    [Space(10)]
    public Image image;
    public Button button;

    public Sprite emptySprite;
    public Sprite fillSprite;
    
    public int rowIndex;//自身的行索引
    public static BoxView Instance(BoxView prefab,
                                   Transform parentTransform,
                                   Transform letterParentTransform,
                                   Vector2 position,
                                   char letter,
                                   int rowIndex)
    {
        BoxView boxView = PoolHelper.Create(prefab);
        boxView.transform
               .AnchoredPosition(position)
                .LocalScale(0)
               .SetParent(parentTransform, false);
        
        boxView.letter = letter;
        boxView.rowIndex = rowIndex;
        boxView._letterTextParentTransform = letterParentTransform;
        return boxView;
    }

    public void DoScaleAnimation(float scale, float delay)
    {
        TaskHelper.Create<CoroutineTask>()
                  .Delay(delay).Do(() =>
                                   {
                                       transform
                                          .DOScale(Vector3.one * scale,0.5f)
                                          .SetEase(Ease.OutBack);
                                   })
                  .Execute();
    }
    private Transform _letterTextParentTransform;
    
    private BoxCoinView _boxCoinView;
    private BoxCrossView _boxCrossView;
    private FillLetterView _fillLetterView;

    public FillLetterView fillLetterView => _fillLetterView;
    public char letter { get; set; }

    private bool _isBible;
    public bool isBible
    {
        get => _isBible;
        set
        {
            _isBible = value;
            if(!_isBible) return;
            
            _boxCrossView = BoxCrossView.Instance(boxCrossViewPrefab, transform);
        }
    }
    
    private bool _isBounus;
    public bool isBounus
    {
        get => _isBounus;
        set
        {
            _isBounus = value;
            if(!_isBounus) return;
            
            _boxCoinView = BoxCoinView.Instance(boxCoinViewPrefab, transform);
        }
    }

    public bool isAnswered { get;  set; }

    private void Awake()
    {
        EventManager.instance.RegistEvent<BoxViewButtonEvent>(OnBoxViewButtonEvent);
    }

    private void OnDestroy()
    {
        EventManager.instance.UnRegistEvent<BoxViewButtonEvent>(OnBoxViewButtonEvent);
    }

    private void OnBoxViewButtonEvent(BoxViewButtonEvent @event)
    {
            ChangeButtonEnable(@event.canClick);
    }

    private void ChangeButtonEnable(bool canclick)
    {
        button.enabled = canclick;
    }
    public void Answer(float delay, SelectedBlockView selectedBlockView)
    {
        this.Delay(delay, () => DoAnswer(selectedBlockView.rectTransform,0.5f));
    }

    private void DoAnswer(SelectedBlockView selectedBlockView)
    {
        isAnswered = true;
        button.enabled = false;
        image.raycastTarget = false;

        if (isBible)
        {
            _boxCrossView.Collect();
            _boxCrossView = null;
        }

        if (isBounus)
        {
            _boxCoinView.Collect();
            _boxCoinView = null;
        }

        _fillLetterView = FillLetterView.Instance(fillLetterViewPrefab,
                                                  transform,
                                                  letter,
                                                  rowIndex);
        _fillLetterView.Fly(selectedBlockView, 0.5f, 0, () => image.sprite = fillSprite);
    }
    public void DoAnswer(RectTransform rectTransform,float duration,Action finish = null)
    {
        isAnswered = true;
        button.enabled = false;
        image.raycastTarget = false;

        if (isBible)
        {
            _boxCrossView.Collect();
            _boxCrossView = null;
        }

        if (isBounus)
        {
            _boxCoinView.Collect();
            _boxCoinView = null;
        }

        _fillLetterView = FillLetterView.Instance(fillLetterViewPrefab,
                                                  transform,
                                                  letter
                                                  ,rowIndex);
        _fillLetterView.Fly(rectTransform, duration, 0, () =>
                                                        {
                                                            image.sprite = fillSprite;
                                                            GameController.GetInstance().crossPanel.CheckComplete();
                                                        });        
        finish?.Invoke();
    }

    public void Shake(float delay)
    {
        _fillLetterView?.Shake(delay);
    }

    public  void OnClick()
    {
        DoAnswer(this.GetComponent<RectTransform>(), 0,ChangeParent);
        
        EventManager.instance.DispatchEvent(FingerDialogEvent.instance.Destroy());
    }

    public override void OnRecycle()
    {
        base.OnRecycle();

        image.sprite = emptySprite;
        button.enabled = true;
        image.raycastTarget = true;
        
        _fillLetterView.Dispose();
        
        _boxCrossView.Dispose();
        _boxCrossView = null;
        
        _boxCoinView.Dispose();
        _boxCoinView = null;
    }

    public void Leaf(Vector3 start, float time, float delay)
    {
        if(isAnswered)
            return;

        isAnswered = true;
        
        Leaf leaf = PoolHelper.Create(leafPrefab);
        leaf.Name(leaf.name)
            .transform.Parent(transform, false)
            .Active();
        
        leaf.Play(start,this.transform.position,time,delay,()=>DoAnswer(this.GetComponent<RectTransform>(),0,ChangeParent));
    }

    public void Hint()
    {        
        if(isAnswered)
            return;

        isAnswered = true;

        DoAnswer(this.GetComponent<RectTransform>(), 0, ChangeParent);
    }
    private void ChangeParent()
    {
        fillLetterView.transform.Parent(GameController.GetInstance().crossPanel.letterTextPanel);
        transform.Parent(GameController.GetInstance().crossPanel.fillBoxPanel);
    }
}