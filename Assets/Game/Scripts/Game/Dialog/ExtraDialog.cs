using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseFramework;
using BaseFramework.UI;
using UnityEngine;
using UnityEngine.UI;

public class ExtraDialog : Dialog
{
    public Button topCloseButton;
    public Button bottonCloseButton;
    public Button claimButton;
    
    public ExtraWord extraWordPrefab;
    public Image progressBar;
    public Text progressText;
    
    public RectTransform wordContent;

    private List<ExtraWord> _extraWords;
    
    private int _currentWordCount;
    private int _rewardTime;
    protected override void Awake()
    {
        base.Awake();
        _extraWords = new List<ExtraWord>();
        topCloseButton.onClick.AddListener(Close);
        bottonCloseButton.onClick.AddListener(Close);
        claimButton.onClick.AddListener(Claim); 
    }

    private void OnEnable()
    {
        base.OnEnable();
        UpdateDialog();
    }

    public void InitWordContent()
    {
        if(!PlayerDataManager.extraInfo.GetWords().Any() || _extraWords.Count == PlayerDataManager.extraInfo.GetWords().Count) return;
        PlayerDataManager.extraInfo.GetWords().ForEach(extraWord =>
                                                       {
                                                           ExtraWord word = PoolHelper
                                                                           .Create(extraWordPrefab).Name(extraWord);
                                                           word.Init(extraWord);
                                                           word.transform.Parent(wordContent, false);
                                                           
                                                           _extraWords.Add(word);
                                                       });
    }

    private void UpdateDialog()
    {
        int extraProgress = PlayerDataManager.extraInfo.extraCurrentCount;
        int extraTargetCount = PlayerDataManager.extraInfo.extraTargetCount;

        progressBar.fillAmount = (extraProgress / (float) extraTargetCount > 1)
                                     ? 1
                                     : (extraProgress / (float) extraTargetCount);
        progressText.text = extraProgress + "/" + extraTargetCount;

        bool canClaim = extraProgress >= extraTargetCount;
        
        bottonCloseButton.gameObject.SetActive(!canClaim);
        claimButton.gameObject.SetActive(canClaim);
        
        //todo 设置百合花状态
        if (canClaim)
        {
            int currentTargetCount = extraTargetCount;
            _currentWordCount = extraProgress;
            int currentGetRewardCount = PlayerDataManager.extraInfo.extraGetRewardCount;
            
            while (_currentWordCount >= currentTargetCount)
            {
                if (currentGetRewardCount < 2)
                {
                    ++currentGetRewardCount;
                }
                else
                {
                    currentTargetCount = 10;
                }

                if (_currentWordCount >= currentTargetCount)
                {
                    _currentWordCount -= currentTargetCount;
                    ++_rewardTime;
                }
            }
        }
    }
    
    private void Claim()
    {
        //todo 金币增加
        PlayerDataManager.extraInfo.extraCurrentCount = _currentWordCount;
        PlayerDataManager.extraInfo.extraGetRewardCount += _rewardTime;

        PlayerDataManager.extraInfo.ClearRewardWord();
        
        
        _extraWords.ForEach((index, it) =>
                            {
                                if (index < PlayerDataManager.extraInfo.extraTargetCount)
                                {
                                    Destroy(it.gameObject);
                                }
                            });
        UpdateDialog(); 
    }
}
