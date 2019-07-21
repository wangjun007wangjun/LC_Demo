using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using BaseFramework;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LetterPanel : MonoBehaviour
{
    public LineController lineControllerPrefab;
    [Space(10)] 
    public LetterBlockView letterBlockViewPrefab;
    public float blockSizeProportion = 0.3f;
    public float blockMaxSize = 200f;
    public float blockTouchScale = 1f;
    [Space(10)]
    public LetterBlockBgView letterBgViewPrefab;
    public Transform letterBgViewParent;

    public Image letterPanelBg;
    
    private LevelBean _levelBean;
    private RectTransform _rectTransform;
    private SingleTouchController _singleTouchController;
    private LineController _lineController;

    private readonly List<Vector3> _blockLocalPositions = new List<Vector3>();
    private readonly List<LetterBlockView> _letterBlocks = new List<LetterBlockView>();
    private readonly List<Vector3> _linePositions = new List<Vector3>();
    private readonly List<LetterBlockView> _selectedBlocks = new List<LetterBlockView>();

    private void Awake()
    {
        _rectTransform = transform as RectTransform;
        _singleTouchController = GetComponent<SingleTouchController>();
        
        EventManager.instance.RegistEvent<CrossPanelEvent>(OnCrossPanelEvent);
    }

    private void OnCrossPanelEvent(CrossPanelEvent @event)
    {
        UpdateByAnswerType(@event.type);
    }

    private void OnDestroy()
    {
        EventManager.instance.UnRegistEvent<CrossPanelEvent>(OnCrossPanelEvent);
    }

    public void Init(LevelBean levelBean)
    {
        this._levelBean = levelBean;

        if (_levelBean == null) return;

        _singleTouchController.Register(OnPointerDown, OnDrag, OnPointerUp);

        CreateLine();
        
        
        //背景盘渐变出现
        letterPanelBg.DOColor(new Color(1, 1, 1, 1), 0.5f).OnComplete(InitView);

        //InitView();
    }

    private void CreateLine()
    {
        if (_lineController != null) return;
        
        _lineController = Instantiate(lineControllerPrefab).Name(lineControllerPrefab.name);
        _lineController.transform.Position(Vector3.zero);
        _lineController.SetRendererColor(_levelBean.color);
    }

    private void InitView()
    {
        // get block size
        Vector2 panelSize = _rectTransform.rect.size;
        float size = Mathf.Min(panelSize.x, panelSize.y) * blockSizeProportion;
        size = Mathf.Min(size, blockMaxSize);

        // reset show content size
        Vector2 temp = Vector2.one * Mathf.Min(panelSize.x, panelSize.y);
        panelSize = temp + (panelSize - temp) / 2;
        // get radius
        panelSize -= Vector2.one * size;
        float radiusX = panelSize.x / 2;
        float radiusY = panelSize.y / 2;
        Log.I(this, "letter content size:{0}, blockSize:{1}, radiusX:{2}, radiusY:{3}",
              panelSize, size, radiusX, radiusY);

        int letterCount = _levelBean.w.Length;

        float letterAngle = Mathf.PI / 2;
        float deltaAngle = 2 * Mathf.PI / letterCount;

        // letter block prefab size, calculate block scale
        float prefabSize = letterBlockViewPrefab.GetComponent<RectTransform>().rect.size.x;

        List<char> shuffleLetters = new List<char>(_levelBean.w);
        // 延迟开始创建LetterBlock
        CoroutineTask coroutineTask = TaskHelper.Create<CoroutineTask>().SetMonoBehaviour(this).Delay(0.1f);
        shuffleLetters.Shuffle();
        shuffleLetters.ForEach((index, letter) =>
                               {
                                   float x = Mathf.Cos(letterAngle);
                                   float y = Mathf.Sin(letterAngle);
                                   Vector3 localPosition = new Vector2(x * radiusX, y * radiusY);
                                   _blockLocalPositions.Add(localPosition);

                                   coroutineTask.Delay(0.1f).Do(() =>
                                                                {
                                                                    LetterBlockView letterBlockView =
                                                                        LetterBlockView.Create(letterBlockViewPrefab,
                                                                                           transform,
                                                                                           letter,
                                                                                           localPosition,
                                                                                           size / prefabSize);


                                                                    _letterBlocks.Add(letterBlockView);
                                                                });

                                   letterAngle += deltaAngle;
                               });
        coroutineTask.Execute();
    }

    private void OnPointerDown(Vector3 pointPosition)
    {
        ClearLine();

        DealOperat(pointPosition);
    }

    private void OnDrag(Vector3 pointPosition)
    {
        _linePositions.Clear();
        
        DealOperat(pointPosition);
    }

    private void OnPointerUp(Vector3 pointPosition)
    {
        if (_selectedBlocks.IsNullOrEmpty()) return;

        StringBuilder wordBuilder = new StringBuilder();
        _selectedBlocks.ForEach((index, block) =>
                                {
                                    wordBuilder.Append(block.letter);
                                    block.isSelected = false;
                                });

        string word = wordBuilder.ToString();
        if (word.Length > 1)
        {
            EventManager.instance.DispatchEvent(LetterPanelEvent.instance.SetWord(word));
        }
        else
        {
            UpdateByAnswerType(EAnswerType.Error);
        }
        EventManager.instance.DispatchEvent(LetterPanelEvent.instance.SetDelay(0.1f));

        ClearLine();
        _selectedBlocks.Clear();
    }
    
    private void DealOperat(Vector3 pointPosition)
    {
        LetterBlockView blockView = GetLetterBlockByPosition(pointPosition);

        if (blockView)
        {
            // 撤销
            if (ShouldRevocationSelectedBlock(blockView))
            {
                RemoveLastLetterBlockView();
            }
            else if (!_selectedBlocks.Contains(blockView))
            {
                SelectedLetterBlockView(blockView);
            }
        }

        if (_selectedBlocks.Count <= 0) return;
        foreach (LetterBlockView item in _selectedBlocks)
        {
            _linePositions.Add(new Vector3(item.transform.position.x, item.transform.position.y, 90));
        }

        Vector3 touchWorldPosition = MainCamera.mainCamera.ScreenToWorldPoint(pointPosition);
        _linePositions.Add(new Vector3(touchWorldPosition.x, touchWorldPosition.y, 90));

        _lineController.DrawSmooth(_linePositions.ToArray());
    }

    private bool ShouldRevocationSelectedBlock(LetterBlockView blockView)
    {
        return _selectedBlocks.Count > 1 && _selectedBlocks[_selectedBlocks.Count - 2] == blockView;
    }

    private void RemoveLastLetterBlockView()
    {
        LetterBlockView removeBlockView = _selectedBlocks[_selectedBlocks.Count - 1];
        removeBlockView.isSelected = false;
        _selectedBlocks.Remove(removeBlockView);
        EventManager.instance.DispatchEvent(LetterPanelEvent.instance.Set(ELetterPanelEventType.RemoveLast));
    }

    private void SelectedLetterBlockView(LetterBlockView blockView)
    {
        blockView.isSelected = true;
        _selectedBlocks.Add(blockView);
        
        LetterBlockBgView letterBlockBgView = LetterBlockBgView.Instance(letterBgViewPrefab,
                                                                         letterBgViewParent,
                                                                         _levelBean.color);

        blockView.letterBlockBgView = letterBlockBgView;

        // 第一次选中单词块
        if (_selectedBlocks.Count == 1)
        {
            EventManager.instance.DispatchEvent(LetterPanelEvent.instance.Set(ELetterPanelEventType.ClearImmediately));
            //GameController.instance.hint.Break();
        }

        EventManager.instance.DispatchEvent(LetterPanelEvent.instance.SetLetter(blockView.letter));
    }
    
    private LetterBlockView GetLetterBlockByPosition(Vector3 pointPosition)
    {
        foreach (LetterBlockView item in _letterBlocks)
        {
            if (item.rectTransform.IsPositionIn(pointPosition, false, blockTouchScale))
            {
                return item;
            }
        }
        return null;
    }
    
    private void UpdateByAnswerType(EAnswerType answerType)
    {
        switch (answerType)
        {
            case EAnswerType.Answer:
                _selectedBlocks.ForEach((index, it) => it.Jelly());
                break;
            case EAnswerType.Error:
                _selectedBlocks.ForEach((index, it) => it.Shake());
                break;
        }
    }
    
    private void ClearLine()
    {
        _linePositions.Clear();
        _lineController.Clear();
    }
    
    public void OnShuffleClick()
    {
        if (!_letterBlocks.Any() || _selectedBlocks.Any()) return;

        //todo 播放声音
        //SoundManager.instance.Play(Const.AssetBundle.Name.SOUND_GAME_SHUFFLE);

        _blockLocalPositions.Shuffle();

        _letterBlocks.ForEach((index, item) =>
                              {
                                  Sequence sequence = DOTween.Sequence();
                                  sequence.Append(item.transform.DOMove(transform.position, 0.5f))
                                          .SetEase(Ease.InQuad)
                                          .Append(item.transform.DOLocalMove(_blockLocalPositions[index], 0.5f))
                                          .SetEase(Ease.OutQuad)
                                          .Insert(0, item.transform.DOLocalRotate(new Vector3(0, 0, 360 * 3),
                                                                                  sequence.Duration(),
                                                                                  RotateMode.FastBeyond360)
                                                         .SetEase(Ease.InOutQuad))
                                          .OnStart(() => _singleTouchController.status =
                                                             SingleTouchController.TouchStatus.Disable)
                                          .OnComplete(() => _singleTouchController.status =
                                                                SingleTouchController.TouchStatus.Idle);
                              });
    }

    /// <summary>
    /// 点击结算弹窗的下一关，操作盘消失过程
    /// </summary>
    public void BgImageHide(Action finish)
    {
        letterPanelBg.DOColor(new Color(1, 1, 1, 0), 0.4f);
        letterPanelBg.transform.DOScale(Vector3.zero, 0.4f);
        this.transform.DOScale(Vector3.zero, 0.4f).OnComplete(()=>finish?.Invoke());
    }
}