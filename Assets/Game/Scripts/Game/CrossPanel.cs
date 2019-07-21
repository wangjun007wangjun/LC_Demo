using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using BaseFramework;
using DG.Tweening;
using UnityEngine;

public class CrossPanel : MonoBehaviour
{
    public float itemRatioSpacing = 0.05f;
    public float itemMaxSize = 80;

    public BoxView boxViewPrefab;

    public Transform emptyBoxPanel;
    public Transform fillBoxPanel;
    public Transform letterTextPanel;

    public ComboPanel comboPanel;

    private LevelBean _levelBean;
    private RectTransform _rectTransform;

    public List<CrossWord> _crossWords;
    public List<BoxView> _allBoxViews;

    public RectTransform leafBtnTransform;
    private void Awake()
    {
        _rectTransform = transform as RectTransform;
        EventManager.instance.RegistEvent<LetterPanelEvent>(OnLetterPanelEvent);
        EventManager.instance.RegistEvent<CrossPanelEvent>(LevelOnComplete);
    }
    
    private void OnLetterPanelEvent(LetterPanelEvent @event)
    {
        if (@event.type == ELetterPanelEventType.Seleceted)
        {
            CheckAnswer(@event.word);
        }
    }

    private void OnDestroy()
    {
        EventManager.instance.UnRegistEvent<LetterPanelEvent>(OnLetterPanelEvent);
        EventManager.instance.UnRegistEvent<CrossPanelEvent>(LevelOnComplete);
    }

    private void LevelOnComplete(CrossPanelEvent @event)
    {
        if (@event.levelOnComplete)
        {
            // Box下落消失
            List<BoxView> boxViews = fillBoxPanel.GetComponentsInChildren<BoxView>().ToList();
            boxViews.Sort((a, b) =>
                          {
                              return b.rowIndex
                               .CompareTo(a.rowIndex);
                          });
            List<FillLetterView> fillLetterViews = letterTextPanel.GetComponentsInChildren<FillLetterView>().ToList();
            fillLetterViews.Sort((a, b) => { return b.rowIndex.CompareTo(a.rowIndex); });
            //test
            float delay = 0f;
            boxViews.ForEach((_, b) =>
                             {
                                 b.transform.DOLocalMoveY(b.GetComponent<RectTransform>().anchoredPosition.y - 250, delay);
                                 b.transform.DOScale(Vector3.zero,delay);
                                 delay += 0.1f;
                             });
            delay = 0;
            fillLetterViews.ForEach((_, b) =>
                             {
                                 b.transform.DOLocalMoveY(b.GetComponent<RectTransform>().anchoredPosition.y - 250, delay);
                                 b.transform.DOScale(Vector3.zero,0.3f);
                                 delay += 0.1f;
                             });
        }
    }
    public void Init(LevelBean levelBean,Action finish)
    {
        _levelBean = levelBean;

        if (_levelBean == null) return;

        InitView(finish);
    }

    private void InitView(Action finish)
    {
        Vector2 contentSize = _rectTransform.rect.size;

        int row = _levelBean.r;
        int col = _levelBean.c;

        // box size
        float colRatio = col + (col - 1) * itemRatioSpacing;
        float rowRatio = row + (row - 1) * itemRatioSpacing;
        float boxWidth = contentSize.x / colRatio;
        float boxHeight = contentSize.y / rowRatio;
        float boxSize = Mathf.Min(Mathf.Min(boxHeight, boxWidth), itemMaxSize);
        Log.I(this, $"boxSize={boxSize}");

        // space size
        float spaceSize = boxSize * itemRatioSpacing;

        // content size
        Vector2 boxContentSize = new Vector2(rowRatio * boxSize, colRatio * boxSize);
        Log.I(this, $"boxContentSize={boxContentSize}");

        // content offset
        Vector2 contentOffset = new Vector2(-boxContentSize.x / 2, boxContentSize.y / 2);
        Log.I(this, $"contentOffset={contentOffset}");

        float boxViewSize = boxViewPrefab.GetComponent<RectTransform>().rect.width;
        float boxViewScale = boxSize / boxViewSize;

        _allBoxViews = new List<BoxView>();
        BoxView[,] boxViews = new BoxView[_levelBean.r, _levelBean.c];
        List<CrossWordBean> crossWordBeans = _levelBean.allCrossWordBeans;

        _crossWords = new List<CrossWord>(crossWordBeans.Count);
        crossWordBeans.ForEach((index, crossWordBean) =>
                               {
                                   bool isH = crossWordBean.direction ==
                                              CrossDirection.Horizontal;

                                   string word = crossWordBean.word;

                                   CrossWord crossWord = new CrossWord(word);
                                   for (int i = 0; i < word.Length; ++i)
                                   {
                                       int rowIndex =
                                           crossWordBean.rowIndex + (isH ? 0 : i);
                                       int colIndex =
                                           crossWordBean.colIndex + (isH ? i : 0);
                                       BoxView boxView = null;
                                       if (boxViews[rowIndex, colIndex] != null)
                                       {
                                           boxView = boxViews[rowIndex, colIndex];
                                       }
                                       else
                                       {
                                           Vector2 boxPosition = contentOffset
                                                                 + new
                                                                     Vector2((boxSize + spaceSize) * colIndex,
                                                                             -(boxSize +
                                                                               spaceSize
                                                                              ) *
                                                                             rowIndex)
                                                                 + new
                                                                     Vector2(boxSize,
                                                                             -boxSize) /
                                                                 2;
                                           boxView = BoxView.Instance(boxViewPrefab,
                                                                      emptyBoxPanel,
                                                                      letterTextPanel,
                                                                      boxPosition,
                                                                      word[i],
                                                                      rowIndex);
                                           boxView.DoScaleAnimation(boxViewScale, rowIndex * 0.1f);
                                           boxViews[rowIndex, colIndex] = boxView;
                                           _allBoxViews.Add(boxView);
                                       }

                                       crossWord.AddBoxView(boxView);

                                       boxView.isBible =
                                           _levelBean.IsBible(crossWordBean);
                                       boxView.isBounus =
                                           _levelBean.IsBonus(crossWordBean);
                                   }

                                   _crossWords.Add(crossWord);
                               });
        TaskHelper.Create<CoroutineTask>().Delay(row * 0.1f)
                  .Do(() => finish?.Invoke())
                  .Execute();
    }

    private void CheckAnswer(string word)
    {
        EAnswerType answerType;
        
        if (_levelBean == null
            || _crossWords.IsNullOrEmpty())
        {
            answerType = EAnswerType.Error;
        }
        else
        {
            CrossWord crossWord = _crossWords.Find(x => x.word == word);

            if (crossWord == null)
            {
                if (_levelBean.IsExtra(word))
                {
                    if (!LevelManager.instance.IsLatestLevel(_levelBean))
                    {
                        answerType = EAnswerType.ExtraClose;
                    }
                    else if (!PlayerDataManager.extraInfo.IsWordExist(word))
                    {
                        answerType = EAnswerType.Extra;
                        //添加extraword
                        PlayerDataManager.extraInfo.AddWord(word);
                    }
                    else
                    {
                        answerType = EAnswerType.InvalidExtra;
                    }
                }
                else
                {
                    answerType = EAnswerType.Error;
                }
            }
            else
            {
                if (crossWord.isAnswered)
                {
                    if(_levelBean.IsBibleWord(word))
                        answerType = EAnswerType.InvalidBible;
                    else if(_levelBean.IsBonusWord(word))
                        answerType = EAnswerType.InvalidBonus;
                    else
                    {
                        answerType = EAnswerType.InvalidAnswer;
                        crossWord.Shake();
                    }
                }
                else
                {
                    if(_levelBean.IsBibleWord(word))
                        answerType = EAnswerType.Bible;
                    else if(_levelBean.IsBonusWord(word))
                        answerType = EAnswerType.Bonus;
                    else
                    {
                        answerType = EAnswerType.Answer;
                    }
                    crossWord.Answer(GameController.GetInstance().selectedPanel.selectedLetters);
                }
            }
        }
        
        EventManager.instance.DispatchEvent(CrossPanelEvent.instance.Set(answerType));
    }

    public void OnLeafClick()
    {
        List<BoxView> unanswerBoxes = _allBoxViews.FindAll(it => !it.isAnswered);

        if (unanswerBoxes.Count() > 5)
        {
            unanswerBoxes.Shuffle();
        }

        this.Delay(0).Do(()=>{
            unanswerBoxes.ForEach((index, box) =>
                                  {
                                      if (index < 5)
                                      {
                                          box.Leaf(leafBtnTransform.GetCenterPosition(),0.4f,0);
                                      }
                                  });
        });
    }

    public void OnHintClick()
    {
        //todo 播放声音
       List<BoxView> unanswerBoxViews =
           (from crossWord in _crossWords
           where !crossWord.boxViews[0].isAnswered
           orderby crossWord.boxViews.Count descending
           select crossWord.boxViews[0]).ToList();
                 
        if (unanswerBoxViews.Any())
        {
            unanswerBoxViews[0].Hint();
            return;
        }
        List<BoxView> unanswerBoxes = _allBoxViews.FindAll(it => !it.isAnswered);
        unanswerBoxes.Shuffle();
        unanswerBoxes[0].Hint();
    }
    
    public void CheckComplete()
    {
        BoxView word = _allBoxViews.Find(x => !x.isAnswered);
        if (word == null)
        {
            GameController.GetInstance().WillComplete();
        
            PlayerDataManager.extraInfo.ClearWords();
            //todo 清空当前关数据
        }
        else
        {
            //todo 储存进度
        }
        
    }
}